using System.Collections.Generic;

namespace Rendering
{
    public partial class RenderingMgr
    {
        public void AddUnitAtFirst(IRenderingNode unit)
        {
            if (unit == null)
                return;
            unit.BaseInit();
            m_llRenderingNodeList.AddFirst(unit);
        }

        public void AddUnitAtLast(IRenderingNode unit)
        {
            if (unit == null)
                return;
            if (m_llRenderingNodeList.Contains(unit))
            {
#if _DEBUG
                UnityEngine.Debug.LogError("Trying to add one unit twice!!!");
                UnityEngine.Debug.LogError(UnityEngine.StackTraceUtility.ExtractStackTrace());
#endif
                return;
            }
            unit.BaseInit();
            m_llRenderingNodeList.AddLast(unit);
        }

        public void AddUnitAfterNode(IRenderingNode unit, LinkedListNode<IRenderingNode> node)
        {
            if (node == null || unit == null)
                return;
            unit.BaseInit();
            m_llRenderingNodeList.AddAfter(node, unit);
        }

        public void AddUnitBeforeNode(IRenderingNode unit, LinkedListNode<IRenderingNode> node)
        {
            if (node == null || unit == null)
                return;
            unit.BaseInit();
            m_llRenderingNodeList.AddBefore(node, unit);
        }

        public void AddCrucialUnitAtFirst(string unitName, IRenderingNode unit)
        {
            if (unit == null)
                return;
            unit.BaseInit();
            LinkedListNode<IRenderingNode> newUnit = new LinkedListNode<IRenderingNode>(unit);
            m_dicCrucialNodes.Add(unitName, newUnit);
            m_llRenderingNodeList.AddFirst(newUnit);
        }

        public void AddCrucialUnitAtLast(string unitName, IRenderingNode unit)
        {
            if (unit == null)
                return;
            unit.BaseInit();
            LinkedListNode<IRenderingNode> newUnit = new LinkedListNode<IRenderingNode>(unit);
            m_dicCrucialNodes.Add(unitName, newUnit);
            m_llRenderingNodeList.AddLast(newUnit);
        }

        public void AddCrucialUnitAfterNode(string unitName, IRenderingNode unit, LinkedListNode<IRenderingNode> node)
        {
            if (node == null || unit == null)
                return;
            unit.BaseInit();
            LinkedListNode<IRenderingNode> newUnit = new LinkedListNode<IRenderingNode>(unit);
            m_dicCrucialNodes.Add(unitName, newUnit);
            m_llRenderingNodeList.AddAfter(node, newUnit);
        }

        public void AddCrucialUnitBeforeNode(string unitName, IRenderingNode unit, LinkedListNode<IRenderingNode> node)
        {
            if (node == null || unit == null)
                return;
            unit.BaseInit();
            LinkedListNode<IRenderingNode> newUnit = new LinkedListNode<IRenderingNode>(unit);
            m_dicCrucialNodes.Add(unitName, newUnit);
            m_llRenderingNodeList.AddBefore(node, newUnit);
        }

        public void RemoveNode(IRenderingNode unit)
        {
            LinkedListNode<IRenderingNode> node = m_llRenderingNodeList.Find(unit);
            if (node == null)
                return;

            node.Value.BaseClear();
            node.Value.Dispose();
            m_llRenderingNodeList.Remove(node);
        }

    }
}
