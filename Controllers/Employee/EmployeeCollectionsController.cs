using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ESPL.KP.Entities;
using ESPL.KP.Helpers;
using ESPL.KP.Helpers.Core;
using ESPL.KP.Models;
using ESPL.KP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace KP.Controllers.Employee
{
     [Route("api/EmployeeCollection")]
    public class EmployeeCollectionsController : Controller
    {
        private ILibraryRepository _libraryRepository;

        public EmployeeCollectionsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpPost]
        [Authorize(Policy = Permissions.EmployeeCreate)]
        public IActionResult CreateEmployeeCollection(
            [FromBody] IEnumerable<EmployeeForCreationDto> EmployeeCollection)
        {
            if (EmployeeCollection == null)
            {
                return BadRequest();
            }

            var EmployeeEntities = Mapper.Map<IEnumerable<MstEmployee>>(EmployeeCollection);

            foreach (var Employee in EmployeeEntities)
            {
                _libraryRepository.AddEmployee(Employee);
            }

            if (!_libraryRepository.Save())
            {
                throw new Exception("Creating an Employee collection failed on save.");
            }

            var EmployeeCollectionToReturn = Mapper.Map<IEnumerable<EmployeeDto>>(EmployeeEntities);
            var idsAsString = string.Join(",",
                EmployeeCollectionToReturn.Select(a => a.EmployeeID));

            return CreatedAtRoute("GetEmployeeCollection",
                new { ids = idsAsString },
                EmployeeCollectionToReturn);
            //return Ok();
        }

        // (key1,key2, ...)

        [HttpGet("({ids})", Name = "GetEmployeeCollection")]
        [Authorize(Policy = Permissions.EmployeeRead)]
        public IActionResult GetEmployeeCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var EmployeeEntities = _libraryRepository.GetEmployees(ids);

            if (ids.Count() != EmployeeEntities.Count())
            {
                return NotFound();
            }

            var EmployeesToReturn = Mapper.Map<IEnumerable<EmployeeDto>>(EmployeeEntities);
            return Ok(EmployeesToReturn);
        }
    }
}