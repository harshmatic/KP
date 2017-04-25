﻿using ESPL.KP.Entities;
using ESPL.KP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESPL.KP.Entities.Core;
using ESPL.KP.Models.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ESPL.KP.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _esplUserPropertyMapping =
          new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
          {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "FirstName", new PropertyMappingValue(new List<string>() { "FirstName" } )},
               { "LastName", new PropertyMappingValue(new List<string>() { "LastName" } )},
               { "Email", new PropertyMappingValue(new List<string>() { "Email" } )},
               { "UserName", new PropertyMappingValue(new List<string>() { "UserName" } )}
          };

        private Dictionary<string, PropertyMappingValue> _authorPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "Genre", new PropertyMappingValue(new List<string>() { "Genre" } )},
               { "Age", new PropertyMappingValue(new List<string>() { "DateOfBirth" } , true) },
               { "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" }) }
           };

        private Dictionary<string, PropertyMappingValue> _departmentPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "DepartmentID", new PropertyMappingValue(new List<string>() { "DepartmentID" } ) },
               { "DepartmentName", new PropertyMappingValue(new List<string>() { "DepartmentName" } )},
               { "DepartmentDespcription", new PropertyMappingValue(new List<string>() { "DepartmentDespcription" } )}
           };

        private Dictionary<string, PropertyMappingValue> _areaPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "AreaID", new PropertyMappingValue(new List<string>() { "AreaID" } ) },
               { "AreaName", new PropertyMappingValue(new List<string>() { "AreaName" } )},
               { "AreaCode", new PropertyMappingValue(new List<string>() { "AreaCode" } )}
           };

        private Dictionary<string, PropertyMappingValue> _designationPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "DesignationID", new PropertyMappingValue(new List<string>() { "DesignationID" } ) },
               { "DesignationName", new PropertyMappingValue(new List<string>() { "DesignationName" } )},
               { "DesignationCode", new PropertyMappingValue(new List<string>() { "DesignationCode" } )}
           };

        private Dictionary<string, PropertyMappingValue> _occurrenctTypePropertyMapping =
        new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
               { "OBTypeID", new PropertyMappingValue(new List<string>() { "OBTypeID" } ) },
               { "OBTypeName", new PropertyMappingValue(new List<string>() { "OBTypeName" } )},
        };

        private Dictionary<string, PropertyMappingValue> _shiftPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "ShiftID", new PropertyMappingValue(new List<string>() { "ShiftID" } ) },
               { "ShiftName", new PropertyMappingValue(new List<string>() { "ShiftName" } )},
                { "StartTime", new PropertyMappingValue(new List<string>() { "StartTime" } )},
                { "EndTime", new PropertyMappingValue(new List<string>() { "EndTime" } )},
           };

        private Dictionary<string, PropertyMappingValue> _statusPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "StatusID", new PropertyMappingValue(new List<string>() { "StatusID" } ) },
               { "StatusName", new PropertyMappingValue(new List<string>() { "StatusName" } )}
           };

        private Dictionary<string, PropertyMappingValue> _occurrencBookPropertyMapping =
     new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
     {
            { "OBID", new PropertyMappingValue(new List<string>() { "OBID" } ) },
            { "AreaID", new PropertyMappingValue(new List<string>() { "AreaID" } ) },
            { "OBTypeID", new PropertyMappingValue(new List<string>() { "OBTypeID" } ) },
            { "DepartmentID", new PropertyMappingValue(new List<string>() { "DepartmentID" } ) },
            { "MstStatus", new PropertyMappingValue(new List<string>() { "MstStatus" } ) },
            { "StatusID", new PropertyMappingValue(new List<string>() { "StatusID" } ) },
            { "OBNumber", new PropertyMappingValue(new List<string>() { "OBNumber" } ) },
            { "OBTime", new PropertyMappingValue(new List<string>() { "OBTime" } ) },
            { "CaseFileNumber", new PropertyMappingValue(new List<string>() { "CaseFileNumber" } ) },
            { "NatureOfOccurrence", new PropertyMappingValue(new List<string>() { "NatureOfOccurrence" } ) },
            { "Remark", new PropertyMappingValue(new List<string>() { "Remark" } ) }
     };

        private Dictionary<string, PropertyMappingValue> _appModulesPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "Name", new PropertyMappingValue(new List<string>() { "Genre" } )},
               { "MenuText", new PropertyMappingValue(new List<string>() { "MenuText" } )}
           };

        private Dictionary<string, PropertyMappingValue> _esplRolesPropertyMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "Name", new PropertyMappingValue(new List<string>() { "Name" } )}
           };

        private Dictionary<string, PropertyMappingValue> _employeePropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
            { "EmployeeID", new PropertyMappingValue(new List<string>() { "EmployeeID" } ) },
            { "FirstName", new PropertyMappingValue(new List<string>() { "FirstName" } ) },
            { "LastName", new PropertyMappingValue(new List<string>() { "LastName" } ) },
            { "EmployeeCode", new PropertyMappingValue(new List<string>() { "EmployeeCode" } ) },
            { "DateofBirth", new PropertyMappingValue(new List<string>() { "DateofBirth" } ) },
            { "Gender", new PropertyMappingValue(new List<string>() { "Gender" } ) },
            { "Mobile", new PropertyMappingValue(new List<string>() { "Mobile" } ) },
            { "Email", new PropertyMappingValue(new List<string>() { "Email" } ) },
            { "ResidencePhone1", new PropertyMappingValue(new List<string>() { "ResidencePhone1" } ) },
            { "OrganizationJoiningDate", new PropertyMappingValue(new List<string>() { "OrganizationJoiningDate" } ) },
            { "ServiceJoiningDate", new PropertyMappingValue(new List<string>() { "ServiceJoiningDate" } ) },
            { "Address1", new PropertyMappingValue(new List<string>() { "Mobile" } ) },
            { "Address2", new PropertyMappingValue(new List<string>() { "Email" } ) },
            { "AreaID", new PropertyMappingValue(new List<string>() { "ResidencePhone1" } ) },
            { "DepartmentID", new PropertyMappingValue(new List<string>() { "DepartmentID" } ) },
            { "DesignationID", new PropertyMappingValue(new List<string>() { "DesignationID" } ) },
            { "ShiftID", new PropertyMappingValue(new List<string>() { "ShiftID" } ) },
            { "UserID", new PropertyMappingValue(new List<string>() { "UserID" } ) }
        };

        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            propertyMappings.Add(new PropertyMapping<ESPLUserDto, ESPLUser>(_esplUserPropertyMapping));
            propertyMappings.Add(new PropertyMapping<AuthorDto, Author>(_authorPropertyMapping));
            propertyMappings.Add(new PropertyMapping<DepartmentDto, MstDepartment>(_departmentPropertyMapping));
            propertyMappings.Add(new PropertyMapping<OccurrenceTypeDto, MstOccurrenceType>(_occurrenctTypePropertyMapping));
            propertyMappings.Add(new PropertyMapping<AreaDto, MstArea>(_areaPropertyMapping));
            propertyMappings.Add(new PropertyMapping<DesignationDto, MstDesignation>(_designationPropertyMapping));
            propertyMappings.Add(new PropertyMapping<ShiftDto, MstShift>(_shiftPropertyMapping));
            propertyMappings.Add(new PropertyMapping<StatusDto, MstStatus>(_statusPropertyMapping));
            propertyMappings.Add(new PropertyMapping<OccurrenceBookDto, MstOccurrenceBook>(_occurrencBookPropertyMapping));
            propertyMappings.Add(new PropertyMapping<AppModuleDto, AppModule>(_appModulesPropertyMapping));
            propertyMappings.Add(new PropertyMapping<ESPLRoleDto, IdentityRole>(_esplRolesPropertyMapping));
            propertyMappings.Add(new PropertyMapping<EmployeeDto, MstEmployee>(_employeePropertyMapping));
        }
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping
            <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is separated by ",", so we split it.
            var fieldsAfterSplit = fields.Split(',');

            // run through the fields clauses
            foreach (var field in fieldsAfterSplit)
            {
                // trim
                var trimmedField = field.Trim();

                // remove everything after the first " " - if the fields 
                // are coming from an orderBy string, this part must be 
                // ignored
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                // find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;

        }

    }
}
