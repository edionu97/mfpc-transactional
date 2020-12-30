using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseSystem.Persistence.Repository.Abstract
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Generic update method
        /// </summary>
        /// <typeparam name="T">the type of the object</typeparam>
        /// <param name="entity">the entity itself</param>
        public Task UpdateAsync(T entity);

        /// <summary>
        /// Adds the element into the database
        /// </summary>
        /// <typeparam name="T">the type of the element</typeparam>
        /// <param name="entity">the element that will be added</param>
        public Task AddAsync(T entity);

        /// <summary>
        /// Deletes the element from the database
        /// </summary>
        /// <typeparam name="T">the type of the element</typeparam>
        /// <param name="entity">the element that will be deleted</param>
        public Task DeleteAsync(T entity);

        /// <summary>
        /// This method returns the object that has a specific id 
        /// </summary>
        /// <param name="id">the value of the id</param>
        /// <param name="fieldsToBeIncluded">virtual fields that will be included</param>
        /// <returns>null if the object does not exist or the object itself</returns>
        public Task<T> FindByIdAsync(int id, IList<string> fieldsToBeIncluded = null);

        /// <summary>
        /// Get all elements from database
        /// </summary>
        /// <param name="fieldsToBeIncluded">virtual fields that will be included</param>
        /// <returns>a list of elements that will be included</returns>
        public Task<IList<T>> GetAllAsync(IList<string> fieldsToBeIncluded = null);
    }
}
