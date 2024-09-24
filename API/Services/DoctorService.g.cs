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
    /// The doctorService responsible for managing doctor related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting doctor information.
    /// </remarks>
    public interface IDoctorService
    {
        /// <summary>Retrieves a specific doctor by its primary key</summary>
        /// <param name="id">The primary key of the doctor</param>
        /// <returns>The doctor data</returns>
        Doctor GetById(Guid id);

        /// <summary>Retrieves a list of doctors based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of doctors</returns>
        List<Doctor> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new doctor</summary>
        /// <param name="model">The doctor data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Doctor model);

        /// <summary>Updates a specific doctor by its primary key</summary>
        /// <param name="id">The primary key of the doctor</param>
        /// <param name="updatedEntity">The doctor data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Doctor updatedEntity);

        /// <summary>Updates a specific doctor by its primary key</summary>
        /// <param name="id">The primary key of the doctor</param>
        /// <param name="updatedEntity">The doctor data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Doctor> updatedEntity);

        /// <summary>Deletes a specific doctor by its primary key</summary>
        /// <param name="id">The primary key of the doctor</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The doctorService responsible for managing doctor related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting doctor information.
    /// </remarks>
    public class DoctorService : IDoctorService
    {
        private TestTodayContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the Doctor class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public DoctorService(TestTodayContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific doctor by its primary key</summary>
        /// <param name="id">The primary key of the doctor</param>
        /// <returns>The doctor data</returns>
        public Doctor GetById(Guid id)
        {
            var entityData = _dbContext.Doctor.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of doctors based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of doctors</returns>/// <exception cref="Exception"></exception>
        public List<Doctor> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetDoctor(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new doctor</summary>
        /// <param name="model">The doctor data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Doctor model)
        {
            model.Id = CreateDoctor(model);
            return model.Id;
        }

        /// <summary>Updates a specific doctor by its primary key</summary>
        /// <param name="id">The primary key of the doctor</param>
        /// <param name="updatedEntity">The doctor data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Doctor updatedEntity)
        {
            UpdateDoctor(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific doctor by its primary key</summary>
        /// <param name="id">The primary key of the doctor</param>
        /// <param name="updatedEntity">The doctor data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Doctor> updatedEntity)
        {
            PatchDoctor(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific doctor by its primary key</summary>
        /// <param name="id">The primary key of the doctor</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteDoctor(id);
            return true;
        }
        #region
        private List<Doctor> GetDoctor(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Doctor.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Doctor>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Doctor), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Doctor, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateDoctor(Doctor model)
        {
            _dbContext.Doctor.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateDoctor(Guid id, Doctor updatedEntity)
        {
            _dbContext.Doctor.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteDoctor(Guid id)
        {
            var entityData = _dbContext.Doctor.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Doctor.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchDoctor(Guid id, JsonPatchDocument<Doctor> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Doctor.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Doctor.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}