using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace AfGD.Assignment3
{
    public enum DrawMode
    {
        Rooms,
        Cells
    }

    public class DungeonGenerator : MonoBehaviour
    {
        // Debug Draw Settings
        [SerializeField] DrawMode m_DrawMode;
        [SerializeField] int m_DrawLevel;

        // Volume in which to generate the dungeon
        [SerializeField] Bounds m_Bounds;

        // Root node of our BSP tree
        Node m_TreeRoot;

        void Awake()
        {
            m_TreeRoot = new Node(m_Bounds);
        }

        void Start()
        {
            Assert.IsNotNull(m_TreeRoot);
            m_TreeRoot.SplitCellRecursively();
            m_TreeRoot.GenerateRoomsRecursively();
            m_TreeRoot.UpdateRoomBoundsRecursively();
        }

        void Update()
        {
            m_TreeRoot.ConnectRoomsRecursively();
        }

        void OnDrawGizmos()
        {
            if (m_TreeRoot == null)
                return;

            var nodes = new List<Node>();
            if (m_DrawLevel == -1)
                m_TreeRoot.GetLeafNodes(nodes);
            else
                m_TreeRoot.GetNodesAtLevel(nodes, m_DrawLevel);

            foreach (var node in nodes)
                node.DebugDraw(m_DrawMode);
        }
    }
}