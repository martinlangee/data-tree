﻿using System;
using System.Linq;
using System.Xml;

namespace DataTreeBase
{
    /// <summary>
    /// Represents a container where sub-containers can be added and removed dynamically
    /// </summary>
    public class  DataTreeDynParentContainer<T> : DataTreeContainer where T: DataTreeContainer
    {
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="parent">Parent container</param>
        /// <param name="id">Container identfication</param>
        /// <param name="name">Container name</param>
        protected DataTreeDynParentContainer(DataTreeContainer parent, string id, string name)
            : base(parent, id, name)
        {
        }

        /// <summary>
        /// Creates a new sub-container in case this is a dynamic container and returns if it is overridden in implementin class
        /// </summary>
        protected T CreateDynChild()
        {
            var type = typeof(T);
            return Activator.CreateInstance(type, this) as T;
        }

        /// <summary>
        /// Adds a new data tree container to the list of sub containers
        /// </summary>
        /// <returns>The new DataTreeContainer</returns>
        public T Add()
        {
            return CreateDynChild();
        }

        /// <summary>
        /// Adds a new data tree container to the list of sub containers
        /// </summary>
        /// <returns>The new DataTreeContainer</returns>
        public T Add(Action<T> initAction)
        {
            var child = CreateDynChild();
            initAction?.Invoke(child);
            return child;
        }

        /// <summary>
        /// Deletes the specified dynamic sub container from the list
        /// </summary>
        public void Remove(T container)
        {
            Containers.Remove(container);
        }

        /// <summary>
        /// Deletes the dynamic sub container at the specified index from the list
        /// </summary>
        public void RemoveAt(int index)
        {
            Containers.RemoveAt(index);
        }

        /// <summary>
        /// Removes all sub containers
        /// </summary>
        public void Clear()
        {
            Containers.Clear();
        }

        /// <summary>
        /// Loads the container and it's sub-containers dynamically from the specified parent xml node
        /// </summary>
        /// <param name="parentXmlNode">The parent node</param>
        public override void LoadFromXml(XmlNode parentXmlNode)
        {
            var xmlNode = parentXmlNode.ChildNodeByTagIdAndIdx(Tagname, Id);
            if (xmlNode == null) return;

            Clear();

            xmlNode.ChildNodes.Cast<XmlNode>().ForEach(ch => CreateDynChild().LoadFromXml(ch));
            Params.ForEach(p => p.LoadFromXml(xmlNode));
        }
    }
}
