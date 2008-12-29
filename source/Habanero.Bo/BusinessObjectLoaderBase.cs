//---------------------------------------------------------------------------------
// Copyright (C) 2008 Chillisoft Solutions
// 
// This file is part of the Habanero framework.
// 
//     Habanero is a free framework: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     The Habanero framework is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

using System;
using System.Collections;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.Util;

namespace Habanero.BO
{
    ///<summary>
    /// A base class for BusinessObjectLoader implementations. This base class covers all the 
    /// handling most of the overload methods and just leaves the Refresh methods to be 
    /// implemented in order to complete the implementation of the BusinessObjectLoader.
    ///</summary>
    public abstract class BusinessObjectLoaderBase
    {
        private static Criteria GetCriteriaObject(IClassDef classDef, string criteriaString)
        {
            Criteria criteria = CriteriaParser.CreateCriteria(criteriaString);
            QueryBuilder.PrepareCriteria(classDef, criteria);
            return criteria;
        }

        #region GetBusinessObjectCollection

        private static void CheckNotTypedAsBusinessObject<T>() where T : class, IBusinessObject, new()
        {
            if (typeof (T) == typeof (BusinessObject))
            {
                throw ExceptionHelper.CreateLoaderGenericTypeMethodException();
            }
        }

        /// <summary>
        /// Loads a BusinessObjectCollection using the criteria given. 
        /// </summary>
        /// <typeparam name="T">The type of collection to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="criteria">The criteria to use to load the business object collection</param>
        /// <returns>The loaded collection</returns>
        public BusinessObjectCollection<T> GetBusinessObjectCollection<T>(Criteria criteria)
            where T : class, IBusinessObject, new()
        {
            CheckNotTypedAsBusinessObject<T>();
            BusinessObjectCollection<T> col = new BusinessObjectCollection<T>();
            col.SelectQuery.Criteria = criteria;
            Refresh(col);
            return col;
        }

        /// <summary>
        /// Loads a BusinessObjectCollection using the criteria given. 
        /// </summary>
        /// <typeparam name="T">The type of collection to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="criteriaString">The criteria to use to load the business object collection</param>
        /// <returns>The loaded collection</returns>
        public BusinessObjectCollection<T> GetBusinessObjectCollection<T>(string criteriaString)
            where T : class, IBusinessObject, new()
        {
            CheckNotTypedAsBusinessObject<T>();
            Criteria criteria = GetCriteriaObject(ClassDef.ClassDefs[typeof (T)], criteriaString);
            //            Criteria criteria = CriteriaParser.CreateCriteria(criteriaString);
            //            QueryBuilder.PrepareCriteria(ClassDef.ClassDefs[typeof(T)], criteria);
            return GetBusinessObjectCollection<T>(criteria);
        }

        /// <summary>
        /// Loads a BusinessObjectCollection using the criteria given. 
        /// </summary>
        /// <param name="classDef">The ClassDef for the collection to load</param>
        /// <param name="criteria">The criteria to use to load the business object collection</param>
        /// <returns>The loaded collection</returns>
        public IBusinessObjectCollection GetBusinessObjectCollection(IClassDef classDef, Criteria criteria)
        {
            IBusinessObjectCollection col = CreateCollectionOfType(classDef.ClassType);
            col.ClassDef = classDef;
            col.SelectQuery.Criteria = criteria;
            Refresh(col);
            return col;
        }

        /// <summary>
        /// Reloads a BusinessObjectCollection using the criteria it was originally loaded with.  You can also change the criteria or order
        /// it loads with by editing its SelectQuery object. The collection will be cleared as such and reloaded (although Added events will
        /// only fire for the new objects added to the collection, not for the ones that already existed).
        /// </summary>
        /// <typeparam name="T">The type of collection to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="collection">The collection to refresh</param>
        public abstract void Refresh<T>(BusinessObjectCollection<T> collection) where T : class, IBusinessObject, new();

        /// <summary>
        /// Reloads a BusinessObjectCollection using the criteria it was originally loaded with.  You can also change the criteria or order
        /// it loads with by editing its SelectQuery object. The collection will be cleared as such and reloaded (although Added events will
        /// only fire for the new objects added to the collection, not for the ones that already existed).
        /// </summary>
        /// <param name="collection">The collection to refresh</param>
        public abstract void Refresh(IBusinessObjectCollection collection);

        /// <summary>
        /// Loads a BusinessObjectCollection using the searchCriteria an given. It's important to make sure that the ClassDef given
        /// has the properties defined in the fields of the select searchCriteria and orderCriteria.  
        /// </summary>
        /// <param name="classDef">The ClassDef for the collection to load</param>
        /// <param name="criteriaString">The select query to use to load from the data source</param>
        /// <param name="orderCriteria">The order that the collections must be loaded in e.g. Surname, FirstName</param>
        /// <returns>The loaded collection</returns>
        public IBusinessObjectCollection GetBusinessObjectCollection
            (IClassDef classDef, string criteriaString, string orderCriteria)
        {
            Criteria criteria = GetCriteriaObject(classDef, criteriaString);
            OrderCriteria orderCriteriaObj = QueryBuilder.CreateOrderCriteria(classDef, orderCriteria);
            return GetBusinessObjectCollection(classDef, criteria, orderCriteriaObj);
        }

        /// <summary>
        /// Loads a BusinessObjectCollection using the searchCriteria an given. It's important to make sure that the ClassDef given
        /// has the properties defined in the fields of the select searchCriteria and orderCriteria.  
        /// </summary>
        /// <param name="classDef">The ClassDef for the collection to load</param>
        /// <param name="criteriaString">The select query to use to load from the data source</param>
        /// <returns>The loaded collection</returns>
        public IBusinessObjectCollection GetBusinessObjectCollection(IClassDef classDef, string criteriaString)
        {
            Criteria criteria = GetCriteriaObject(classDef, criteriaString);
            return GetBusinessObjectCollection(classDef, criteria);
        }


        /// <summary>
        /// Loads a BusinessObjectCollection using the criteria given, applying the order criteria to order the collection that is returned. 
        /// </summary>
        /// <typeparam name="T">The type of collection to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="criteriaString">The criteria to use to load the business object collection</param>
        /// <returns>The loaded collection</returns>
        /// <param name="orderCriteria">The order criteria to use (ie what fields to order the collection on)</param>
        public BusinessObjectCollection<T> GetBusinessObjectCollection<T>(string criteriaString, string orderCriteria)
            where T : class, IBusinessObject, new()
        {
            CheckNotTypedAsBusinessObject<T>();
            ClassDef classDef = ClassDef.ClassDefs[typeof (T)];
            Criteria criteria = GetCriteriaObject(classDef, criteriaString);
            OrderCriteria orderCriteriaObj = QueryBuilder.CreateOrderCriteria(classDef, orderCriteria);
            return GetBusinessObjectCollection<T>(criteria, orderCriteriaObj);
        }

        /// <summary>
        /// Loads a BusinessObjectCollection using the criteria given, applying the order criteria to order the collection that is returned. 
        /// </summary>
        /// <typeparam name="T">The type of collection to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="criteria">The criteria to use to load the business object collection</param>
        /// <returns>The loaded collection</returns>
        /// <param name="orderCriteria">The order criteria to use (ie what fields to order the collection on)</param>
        public BusinessObjectCollection<T> GetBusinessObjectCollection<T>
            (Criteria criteria, OrderCriteria orderCriteria) where T : class, IBusinessObject, new()
        {
            CheckNotTypedAsBusinessObject<T>();
            BusinessObjectCollection<T> col = new BusinessObjectCollection<T>();
            col.SelectQuery.Criteria = criteria;
            col.SelectQuery.OrderCriteria = orderCriteria;
            Refresh(col);
            return col;
        }

        /// <summary>
        /// Loads a BusinessObjectCollection using the criteria given, applying the order criteria to order the collection that is returned. 
        /// </summary>
        /// <param name="classDef">The ClassDef for the collection to load</param>
        /// <param name="criteria">The criteria to use to load the business object collection</param>
        /// <returns>The loaded collection</returns>
        /// <param name="orderCriteria">The order criteria to use (ie what fields to order the collection on)</param>
        public IBusinessObjectCollection GetBusinessObjectCollection
            (IClassDef classDef, Criteria criteria, OrderCriteria orderCriteria)
        {
            IBusinessObjectCollection col = CreateCollectionOfType(classDef.ClassType);
            col.ClassDef = classDef;
            QueryBuilder.PrepareCriteria(classDef, criteria);
            col.SelectQuery.Criteria = criteria;
            col.SelectQuery.OrderCriteria = orderCriteria;
            Refresh(col);
            return col;
        }

        /// <summary>
        /// Loads a BusinessObjectCollection using the SelectQuery given. It's important to make sure that T (meaning the ClassDef set up for T)
        /// has the properties defined in the fields of the select query.  
        /// This method allows you to define a custom query to load a businessobjectcollection so that you can perhaps load from multiple
        /// tables using a join (if loading from a database source).
        /// </summary>
        /// <typeparam name="T">The type of collection to load. This must be a class that implements IBusinessObject and has a parameterless constructor</typeparam>
        /// <param name="selectQuery">The select query to use to load from the data source</param>
        /// <returns>The loaded collection</returns>
        public BusinessObjectCollection<T> GetBusinessObjectCollection<T>(ISelectQuery selectQuery)
            where T : class, IBusinessObject, new()
        {
            CheckNotTypedAsBusinessObject<T>();
            BusinessObjectCollection<T> col = new BusinessObjectCollection<T>();
            col.SelectQuery = selectQuery;
            Refresh(col);
            return col;
        }

        /// <summary>
        /// Loads a BusinessObjectCollection using the SelectQuery given. It's important to make sure that the ClassDef given
        /// has the properties defined in the fields of the select query.  
        /// This method allows you to define a custom query to load a businessobjectcollection so that you can perhaps load from multiple
        /// tables using a join (if loading from a database source).
        /// </summary>
        /// <param name="classDef">The ClassDef for the collection to load</param>
        /// <param name="selectQuery">The select query to use to load from the data source</param>
        /// <returns>The loaded collection</returns>
        public IBusinessObjectCollection GetBusinessObjectCollection(IClassDef classDef, ISelectQuery selectQuery)
        {
            IBusinessObjectCollection col = CreateCollectionOfType(classDef.ClassType);
            col.ClassDef = classDef;
            col.SelectQuery = selectQuery;
            Refresh(col);
            return col;
        }

        #endregion

        protected static IBusinessObjectCollection CreateCollectionOfType(Type BOType)
        {
            Type boColType = typeof (BusinessObjectCollection<>).MakeGenericType(BOType);
            return (IBusinessObjectCollection) Activator.CreateInstance(boColType);
        }

//        protected static void AddBusinessObjectToCollection<T>
//            (BusinessObjectCollection<T> collection, T loadedBo, BusinessObjectCollection<T> clonedCol)
//            where T : class, IBusinessObject, new()
//        {
//            //If the origional collection had the new business object then
//            // use add internal this adds without any events being raised etc.
//            //else adds via the Add method (normal add) this raises events such that the 
//            // user interface can be updated.
//            if (clonedCol.Contains(loadedBo))
//            {
//                ((IBusinessObjectCollection) collection).AddWithoutEvents(loadedBo);
//            }
//            else
//            {
//                collection.Add(loadedBo);
//            }
//            collection.PersistedBOCol.Add(loadedBo);
//        }

        protected static void AddBusinessObjectToCollection
            (IBusinessObjectCollection collection, IBusinessObject loadedBo)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (loadedBo == null) throw new ArgumentNullException("loadedBo");
            //If the origional collection had the new business object then
            // use add internal this adds without any events being raised etc.
            //else adds via the Add method (normal add) this raises events such that the 
            // user interface can be updated.
            if (collection.AddedBOCol.Contains(loadedBo))
            {
                collection.AddWithoutEvents(loadedBo);
                collection.PersistedBOCol.Add(loadedBo);
                return;
            }
            if (collection.PersistedBOCol.Contains(loadedBo))
            {
                collection.AddWithoutEvents(loadedBo);
                return;
            }
            collection.PersistedBOCol.Add(loadedBo);
            ReflectionUtilities.SetPrivatePropertyValue(collection, "Loading", true);
            collection.Add(loadedBo);
            ReflectionUtilities.SetPrivatePropertyValue(collection, "Loading", false);
        }

        //The collection should show all loaded object less removed or deleted object not yet persisted
        //     plus all created or added objects not yet persisted.
        //Note: This behaviour is fundamentally different than the business objects behaviour which 
        //  throws and error if any of the items are dirty when it is being refreshed.
        //Should a refresh be allowed on a dirty collection (what do we do with BO's
        //TODO: I think this could be done via reflection instead of having all these public methods.
        //   especially where done via the interface.
        //  the other option would be for the business object collection to have another method (other than clone)
        //   that returns another type of object that has these methods to eliminate all these 
        //   public accessors
        protected static void RestoreEditedLists(IBusinessObjectCollection collection)
        {
            ArrayList addedBoArray = new ArrayList();
            addedBoArray.AddRange(collection.AddedBOCol);
            
            RestoreCreatedCollection(collection);
            RestoreRemovedCollection(collection);
            RestoreMarkForDeleteCollection(collection);
            RestoreAddedCollection(collection, addedBoArray);
        }
        protected static void RestoreAddedCollection(IBusinessObjectCollection collection, ArrayList addedBoArray)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            if (addedBoArray == null) throw new ArgumentNullException("addedBoArray");
            //The collection should show all loaded object less removed or deleted object not yet persisted
           //      plus all created or added objects not yet persisted.
            //Note: This behaviour is fundamentally different than the business objects behaviour which 
            //  throws and error if any of the items are dirty when it is being refreshed.
            //Should a refresh be allowed on a dirty collection (what do we do with BO's
            foreach (IBusinessObject addedBO in addedBoArray)
            {
                if (!collection.Contains(addedBO) && !collection.MarkForDeletionBOCol.Contains(addedBO))
                {
                    collection.AddWithoutEvents(addedBO);
                }
                if (!collection.AddedBOCol.Contains(addedBO))
                {
                    collection.AddedBOCol.Add(addedBO);
                }
            }
        }

        private static void RestoreMarkForDeleteCollection(IBusinessObjectCollection collection)
        {
            foreach (BusinessObject businessObject in collection.MarkForDeletionBOCol)
            {
                collection.Remove(businessObject);
                collection.RemovedBOCol.Remove(businessObject);
            }            
        }

        private static void RestoreRemovedCollection(IBusinessObjectCollection collection)
        {
            foreach (BusinessObject businessObject in collection.RemovedBOCol)
            {
                collection.Remove(businessObject);
            }
        }

        private static void RestoreCreatedCollection(IBusinessObjectCollection collection)
        {
            foreach (BusinessObject businessObject in collection.CreatedBOCol)
            {
                collection.AddWithoutEvents(businessObject);
            }
        }

        protected static void RestoreCreatedCollection
            (IBusinessObjectCollection collection, IList createdBusinessObjects)
        {
            //The collection should show all loaded object less removed or deleted object not yet persisted
            //     plus all created or added objects not yet persisted.
            //Note: This behaviour is fundamentally different than the business objects behaviour which 
            //  throws and error if any of the items are dirty when it is being refreshed.
            //Should a refresh be allowed on a dirty collection (what do we do with BO's
            foreach (IBusinessObject createdBO in createdBusinessObjects)
            {
                collection.CreatedBOCol.Add(createdBO);
                collection.AddWithoutEvents(createdBO);
            }
        }

        protected static void RestoreRemovedCollection
            (IBusinessObjectCollection collection, IList removedBusinessObjects)
        {
            //The collection should show all loaded object less removed or deleted object not yet persisted
            //     plus all created or added objects not yet persisted.
            //Note: This behaviour is fundamentally different than the business objects behaviour which 
            //  throws and error if any of the items are dirty when it is being refreshed.
            //Should a refresh be allowed on a dirty collection (what do we do with BO's
            foreach (IBusinessObject removedBO in removedBusinessObjects)
            {
                collection.Remove(removedBO);
            }
        }

//        /// <summary>
//        /// Ensures that the added collection and main collection are synchronised after a refresh.
//        /// </summary>
//        /// <param name="collection">the main bo collection</param>
//        /// <param name="addedBusinessObjects">The list of added BO's prior to loading</param>
//        protected static void RestoreAddedCollection(IBusinessObjectCollection collection, IList addedBusinessObjects)
//        {
//            //The collection should show all loaded object less removed or deleted object not yet persisted
//            //     plus all created or added objects not yet persisted.
//            //Note: This behaviour is fundamentally different than the business objects behaviour which 
//            //  throws and error if any of the items are dirty when it is being refreshed.
//            //Should a refresh be allowed on a dirty collection (what do we do with BO's
//            foreach (IBusinessObject addedBO in addedBusinessObjects)
//            {
//                if (collection.Contains(addedBO)) continue;
//                collection.Add(addedBO);
////                collection.AddedBOCol.Add(addedBO);
//            }
//        }
    }
}