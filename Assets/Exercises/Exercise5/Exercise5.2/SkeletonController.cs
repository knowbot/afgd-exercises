using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AfGD.Exercise5
{

    public class SkeletonController : MonoBehaviour
    {
        // Skeleton hierarchy
        [Header("Have a look in the data structures below")]
        public Skeleton skeleton;
        // Used to store the current pose of the joints in local space. We prefer local space
        // because it makes it easier to manipulate and interpolate the values in each joint
        public AnimationPose currentPose;

        // Transformations in model space, used to store the pose of each joint in 
        // model space and to draw the character. 
        // Normally updated every frame to reflect changes in the pose of the cahracter.
        [HideInInspector]
        public Matrix4x4[] modelSpacePose;

        // Used to store a copy of the mesh
        private TriangleMesh modelMesh;

        // Components used to draw the character mesh. We use a special mesh with
        // colors so that we can visualize how each joint affect the vertices in the mesh
        private MeshRenderer weightMeshRenderer;
        private MeshFilter weightMeshFilter;


        [Header("Debug controls")]
        public bool drawSkeleton = true;
        public bool drawWireframe = true;
        public bool drawWeightMesh = true;

        [Header("Load options")]
        // File where a serialized TriangleMesh object
        public string meshFile = "Exercises/Exercise5/Data/modelMesh.json";
        // File where a serialized Skeleton object 
        public string skeletonFile = "Exercises/Exercise5/Data/modelSkeleton.json";
        // File where an serialized AnimationPose object 
        public string poseFile = "Exercises/Exercise5/Data/modelBindPose.json";

        [Header("Save options")]
        // File where the values of currentPose should be serialized
        public string savePoseFile = "Exercises/Exercise5/Data/newPose.json";

        // To ensure that we initialize this only once
        private bool initialized = false;

        private void Start()
        {
            Init();
        }

        // Add the option to call this function from the right click menu
        [ContextMenu("Call Init")]
        public void Init()
        {
            // Load character mesh (saved as a serialized data structure)
            string meshLoadPath = Application.dataPath + "/" + meshFile;
            string meshString = System.IO.File.ReadAllText(meshLoadPath);
            modelMesh = JsonUtility.FromJson<TriangleMesh>(meshString);

            // Setup this game object to show the mesh, use control weights as colors
            weightMeshRenderer = gameObject.GetComponent<MeshRenderer>();
            if(weightMeshRenderer == null)
                weightMeshRenderer = gameObject.AddComponent<MeshRenderer>();
            weightMeshFilter = gameObject.GetComponent<MeshFilter>();
            if (weightMeshFilter == null)
                weightMeshFilter = gameObject.AddComponent<MeshFilter>();

            // Set the character mesh
            weightMeshFilter.sharedMesh = modelMesh.GetUnityMesh();
            // Assumes that there is a material called WeightsMaterial in a Resources folder
            // a custom material is used so that we can render a mesh using the colors we set
            Material weightsMat = Resources.Load("WeightsMaterial", typeof(Material)) as Material;
            weightMeshRenderer.sharedMaterial = weightsMat;


            // Load skeleton from file (saved as a serialized data structure)
            string skelLoadPath = Application.dataPath + "/" + skeletonFile;
            string skelString = System.IO.File.ReadAllText(skelLoadPath);
            skeleton = JsonUtility.FromJson<Skeleton>(skelString);


            // Load pose from file (saved as a serialized data structure)
            string poseLoadPath = Application.dataPath + "/" + poseFile;
            string poseString = System.IO.File.ReadAllText(poseLoadPath);
            currentPose = JsonUtility.FromJson<AnimationPose>(poseString);


            // Compute the pose of all joints in model space
            skeleton.GetPoseInGlobalSpace(currentPose, ref modelSpacePose);
            // Compute the pose of the vertices of the mesh
            UpdateMeshVertices(modelMesh, modelSpacePose, skeleton, weightMeshFilter.sharedMesh);

            initialized = true;
        }


        private void Update()
        {
            if (!initialized)
                Init();

            // update our matrices
            skeleton.GetPoseInGlobalSpace(currentPose, ref modelSpacePose);

            // update the vertices base on current pose
            UpdateMeshVertices(modelMesh, modelSpacePose, skeleton, weightMeshFilter.sharedMesh);

            // the model matrix
            Matrix4x4 modelMat = this.transform.localToWorldMatrix;


            if (drawSkeleton)
            {
                for (int i = 0; i < modelSpacePose.Length; i++)
                {
                    // draw the perpendicular axis to represent the joint
                    DebugDraw.DrawFrame(modelMat * modelSpacePose[i]);

                    // draw the bone connecting the joints
                    byte parentId = skeleton.m_aJoint[i].m_iParent;
                    DebugDraw.DrawBone(modelMat * modelSpacePose[i],
                    modelMat * (parentId == 255 ? Matrix4x4.identity : modelSpacePose[parentId]));
                }
            }

            if (drawWireframe)
            {
                // draw the wireframe (triangle borders) of the character mesh
                Mesh sharedMesh = weightMeshFilter.sharedMesh;
                Gizmos.color = Color.yellow; // wireframe color
                Gizmos.DrawWireMesh(sharedMesh, this.transform.position, this.transform.rotation);
            }

            // toggle render component enabled/disabled
            if (drawWeightMesh && !weightMeshRenderer.enabled ||
                !drawWeightMesh && weightMeshRenderer.enabled)
            {
                weightMeshRenderer.enabled = !weightMeshRenderer.enabled;
            }

        }

        /// <summary>
        /// Used to render our debug objects while not in play mode
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                Update();
            }
        }



        /// <summary>
        /// Update vertices of a unity mesh
        /// </summary>
        /// <param name="modelMesh">The reference mesh, it should not be modified </param>
        /// <param name="globalPose">the pose of the avatar in model space, we use it to set the vertices</param>
        /// <param name="skeleton">the skeleton containing the inverse binding poses of each joint</param>
        /// <param name="renderMesh">the mesh that we want to update</param>
        private void UpdateMeshVertices(TriangleMesh modelMesh, Matrix4x4[] globalPose, Skeleton skeleton, Mesh renderMesh)
        {
            // KEEP THIS Copy vertices to an array, 
            // we will be setting the position of these vertices
            Vector3[] vertices = renderMesh.vertices;
            // Check if the modelMesh and renderMesh match
            if(vertices.Length != modelMesh.vertices.Length)
                Debug.LogError("[SkeletonController] the render renderMesh and the modelMesh don't have the same number of vertices!");

            // TODO exercise 5.3 
            // Transform the mesh using the bind pose and current transformation of each joint
            // For each vertex in the mesh
            // 1. use modelMesh to retrieve the original position and BoneWeight of each vertex
            // 2. use the bone index in BoneWeight to apply the correct transformations (there are 4 per vertex) to each vertex
            // 3. use the BoneWeight weights to compute the weighted average position of the vertex 
            // (you won't need the renderMesh object)
            // IMPLEMENTATION HERE (sldies 59, 62 and 63)

            // KEEP THIS
            // update the vertices of the unity Mesh
            renderMesh.vertices = vertices;
        }


        public void SaveCurrentPose()
        {
            string poseString = JsonUtility.ToJson(currentPose, true);
            string poseSavePath = Application.dataPath + "/" + savePoseFile;
            System.IO.File.WriteAllText(poseSavePath, poseString);
        }

    }
}
