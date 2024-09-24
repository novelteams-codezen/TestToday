using TestToday.Models;
using TestToday.Data;
using TestToday.Filter;
using TestToday.Entities;
using TestToday.Logger;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;

namespace TestToday.Services
{
    /// <summary>
    /// The dayvisitService responsible for managing dayvisit related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting dayvisit information.
    /// </remarks>
    public interface IDayVisitService
    {
        /// <summary>Retrieves a specific dayvisit by its primary key</summary>
        /// <param name="id">The primary key of the dayvisit</param>
        /// <returns>The dayvisit data</returns>
        DayVisit GetById(Guid id);

        /// <summary>Retrieves a list of dayvisits based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of dayvisits</returns>
        List<DayVisit> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new dayvisit</summary>
        /// <param name="model">The dayvisit data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(DayVisit model);

        /// <summary>Updates a specific dayvisit by its primary key</summary>
        /// <param name="id">The primary key of the dayvisit</param>
        /// <param name="updatedEntity">The dayvisit data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, DayVisit updatedEntity);

        /// <summary>Updates a specific dayvisit by its primary key</summary>
        /// <param name="id">The primary key of the dayvisit</param>
        /// <param name="updatedEntity">The dayvisit data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<DayVisit> updatedEntity);

        /// <summary>Deletes a specific dayvisit by its primary key</summary>
        /// <param name="id">The primary key of the dayvisit</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The dayvisitService responsible for managing dayvisit related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting dayvisit information.
    /// </remarks>
    public class DayVisitService : IDayVisitService
    {
        private TestTodayContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the DayVisit class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public DayVisitService(TestTodayContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific dayvisit by its primary key</summary>
        /// <param name="id">The primary key of the dayvisit</param>
        /// <returns>The dayvisit data</returns>
        public DayVisit GetById(Guid id)
        {
            var entityData = _dbContext.DayVisit.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of dayvisits based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of dayvisits</returns>/// <exception cref="Exception"></exception>
        public List<DayVisit> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetDayVisit(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new dayvisit</summary>
        /// <param name="model">The dayvisit data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(DayVisit model)
        {
            model.Id = CreateDayVisit(model);
            return model.Id;
        }

        /// <summary>Updates a specific dayvisit by its primary key</summary>
        /// <param name="id">The primary key of the dayvisit</param>
        /// <param name="updatedEntity">The dayvisit data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, DayVisit updatedEntity)
        {
            UpdateDayVisit(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific dayvisit by its primary key</summary>
        /// <param name="id">The primary key of the dayvisit</param>
        /// <param name="updatedEntity">The dayvisit data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<DayVisit> updatedEntity)
        {
            PatchDayVisit(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific dayvisit by its primary key</summary>
        /// <param name="id">The primary key of the dayvisit</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteDayVisit(id);
            return true;
        }
        #region
        private List<DayVisit> GetDayVisit(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.DayVisit.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<DayVisit>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(DayVisit), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<DayVisit, object>>(Expression.Convert(property, typeof(object)), parameter);
                if (sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderBy(lambda);
                }
                else if (sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderByDescending(lambda);
                }
                else
                {
                    throw new ApplicationException("Invalid sort order. Use 'asc' or 'desc'");
                }
            }

            var paginatedResult = result.Skip(skip).Take(pageSize).ToList();
            return paginatedResult;
        }

        private Guid CreateDayVisit(DayVisit model)
        {
            _dbContext.DayVisit.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateDayVisit(Guid id, DayVisit updatedEntity)
        {
            _dbContext.DayVisit.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteDayVisit(Guid id)
        {
            var entityData = _dbContext.DayVisit.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.DayVisit.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchDayVisit(Guid id, JsonPatchDocument<DayVisit> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.DayVisit.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.DayVisit.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}