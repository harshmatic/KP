using ESPL.KP.Models;
using ESPL.KP.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using ESPL.KP.Helpers;
using AutoMapper;
using ESPL.KP.Entities;
using Microsoft.AspNetCore.Http;
using ESPL.KP.Helpers.Core;
using ESPL.KP.Helpers.Status;
using Microsoft.AspNetCore.JsonPatch;

namespace ESPL.KP.Controllers.Status
{
    [Route("api/statuses")]
    public class StatusesController : Controller
    {
        private ILibraryRepository _libraryRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public StatusesController(ILibraryRepository libraryRepository,
            IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _libraryRepository = libraryRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetStatuses")]
        [HttpHead]
        public IActionResult GetStatuses(StatusesResourceParameters statusesResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<StatusDto, MstStatus>
               (statusesResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<StatusDto>
                (statusesResourceParameters.Fields))
            {
                return BadRequest();
            }

            var statusesFromRepo = _libraryRepository.GetStatuses(statusesResourceParameters);

            var statuses = Mapper.Map<IEnumerable<StatusDto>>(statusesFromRepo);

            if (mediaType == "application/vnd.marvin.hateoas+json")
            {
                var paginationMetadata = new
                {
                    totalCount = statusesFromRepo.TotalCount,
                    pageSize = statusesFromRepo.PageSize,
                    currentPage = statusesFromRepo.CurrentPage,
                    totalPages = statusesFromRepo.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateLinksForStatuses(statusesResourceParameters,
                    statusesFromRepo.HasNext, statusesFromRepo.HasPrevious);

                var shapedStatuses = statuses.ShapeData(statusesResourceParameters.Fields);

                var shapedStatusesWithLinks = shapedStatuses.Select(status =>
                {
                    var statusAsDictionary = status as IDictionary<string, object>;
                    var statusLinks = CreateLinksForStatus(
                        (Guid)statusAsDictionary["Id"], statusesResourceParameters.Fields);

                    statusAsDictionary.Add("links", statusLinks);

                    return statusAsDictionary;
                });

                var linkedCollectionResource = new
                {
                    value = shapedStatusesWithLinks,
                    links = links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                var previousPageLink = statusesFromRepo.HasPrevious ?
                    CreateStatusesResourceUri(statusesResourceParameters,
                    ResourceUriType.PreviousPage) : null;

                var nextPageLink = statusesFromRepo.HasNext ?
                    CreateStatusesResourceUri(statusesResourceParameters,
                    ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    previousPageLink = previousPageLink,
                    nextPageLink = nextPageLink,
                    totalCount = statusesFromRepo.TotalCount,
                    pageSize = statusesFromRepo.PageSize,
                    currentPage = statusesFromRepo.CurrentPage,
                    totalPages = statusesFromRepo.TotalPages
                };

                Response.Headers.Add("X-Pagination",
                    Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

                return Ok(statuses.ShapeData(statusesResourceParameters.Fields));
            }
        }

        private string CreateStatusesResourceUri(
            StatusesResourceParameters statusesResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetStatuses",
                      new
                      {
                          fields = statusesResourceParameters.Fields,
                          orderBy = statusesResourceParameters.OrderBy,
                          searchQuery = statusesResourceParameters.SearchQuery,
                          pageNumber = statusesResourceParameters.PageNumber - 1,
                          pageSize = statusesResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetStatuses",
                      new
                      {
                          fields = statusesResourceParameters.Fields,
                          orderBy = statusesResourceParameters.OrderBy,
                          searchQuery = statusesResourceParameters.SearchQuery,
                          pageNumber = statusesResourceParameters.PageNumber + 1,
                          pageSize = statusesResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetStatuses",
                    new
                    {
                        fields = statusesResourceParameters.Fields,
                        orderBy = statusesResourceParameters.OrderBy,
                        searchQuery = statusesResourceParameters.SearchQuery,
                        pageNumber = statusesResourceParameters.PageNumber,
                        pageSize = statusesResourceParameters.PageSize
                    });
            }
        }

        [HttpGet("{id}", Name = "GetStatus")]
        public IActionResult GetStatus(Guid id, [FromQuery] string fields)
        {
            if (!_typeHelperService.TypeHasProperties<StatusDto>
              (fields))
            {
                return BadRequest();
            }

            var statusFromRepo = _libraryRepository.GetStatus(id);

            if (statusFromRepo == null)
            {
                return NotFound();
            }

            var status = Mapper.Map<StatusDto>(statusFromRepo);

            var links = CreateLinksForStatus(id, fields);

            var linkedResourceToReturn = status.ShapeData(fields)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }

        [HttpPost(Name = "CreateStatus")]
        public IActionResult CreateStatus([FromBody] StatusForCreationDto status)
        {
            if (status == null)
            {
                return BadRequest();
            }

            var statusEntity = Mapper.Map<MstStatus>(status);

            _libraryRepository.AddStatus(statusEntity);

            if (!_libraryRepository.Save())
            {
                throw new Exception("Creating an status failed on save.");
                // return StatusCode(500, "A problem happened with handling your request.");
            }

            var statusToReturn = Mapper.Map<StatusDto>(statusEntity);

            var links = CreateLinksForStatus(statusToReturn.StatusID, null);

            var linkedResourceToReturn = statusToReturn.ShapeData(null)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetStatus",
                new { id = linkedResourceToReturn["StatusID"] },
                linkedResourceToReturn);
        }

        [HttpPost("{id}")]
        public IActionResult BlockStatusCreation(Guid id)
        {
            if (_libraryRepository.StatusExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpDelete("{id}", Name = "DeleteStatus")]
        public IActionResult DeleteStatus(Guid id)
        {
            var statusFromRepo = _libraryRepository.GetStatus(id);
            if (statusFromRepo == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteStatus(statusFromRepo);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Deleting department {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPut("{id}", Name = "UpdateStatus")]
        public IActionResult UpdateStatus(Guid id, [FromBody] StatusForUpdationDto status)
        {
            if (status == null)
            {
                return BadRequest();
            }
            // if (!_libraryRepository.OccurrenceBookExists(id))
            // {
            //     return NotFound();
            // }
            //Mapper.Map(source,destination);
            var statusRepo = _libraryRepository.GetStatus(id);

            if (statusRepo == null)
            {
                // var statusAdd = Mapper.Map<MstStatus>(status);
                // statusAdd.StatusID = id;

                // _libraryRepository.AddStatus(statusAdd);

                // if (!_libraryRepository.Save())
                // {
                //     throw new Exception($"Upserting status {id} failed on save.");
                // }

                // var statusReturnVal = Mapper.Map<StatusDto>(statusAdd);

                // return CreatedAtRoute("GetStatus",
                //     new { StatusID = statusReturnVal.StatusID },
                //     statusReturnVal);
                return NotFound();
            }
            SetItemHistoryData(status, statusRepo);
            Mapper.Map(status, statusRepo);
            _libraryRepository.UpdateStatus(statusRepo);
            if (!_libraryRepository.Save())
            {
                throw new Exception("Updating an status failed on save.");
                // return StatusCode(500, "A problem happened with handling your request.");
            }


            return Ok(statusRepo);
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateStatus")]
        public IActionResult PartiallyUpdateStatus(Guid id,
                    [FromBody] JsonPatchDocument<StatusForCreationDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var statusFromRepo = _libraryRepository.GetStatus(id);

            if (statusFromRepo == null)
            {
                var statusDto = new StatusForCreationDto();
                patchDoc.ApplyTo(statusDto, ModelState);

                TryValidateModel(statusDto);

                if (!ModelState.IsValid)
                {
                    return new UnprocessableEntityObjectResult(ModelState);
                }

                var statusToAdd = Mapper.Map<MstStatus>(statusDto);
                statusToAdd.StatusID = id;

                _libraryRepository.AddStatus(statusToAdd);

                if (!_libraryRepository.Save())
                {
                    throw new Exception($"Upserting in status {id} failed on save.");
                }

                var statusToReturn = Mapper.Map<StatusDto>(statusToAdd);
                return CreatedAtRoute("GetStatus",
                    new { StatusID = statusToReturn.StatusID },
                    statusToReturn);
            }

            var statusToPatch = Mapper.Map<StatusForCreationDto>(statusFromRepo);

            patchDoc.ApplyTo(statusToPatch, ModelState);

            // patchDoc.ApplyTo(statusToPatch);

            TryValidateModel(statusToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(statusToPatch, statusFromRepo);

            _libraryRepository.UpdateStatus(statusFromRepo);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Patching  status {id} failed on save.");
            }

            return NoContent();
        }

        private IEnumerable<LinkDto> CreateLinksForStatus(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                  new LinkDto(_urlHelper.Link("GetStatus", new { id = id }),
                  "self",
                  "GET"));
            }
            else
            {
                links.Add(
                  new LinkDto(_urlHelper.Link("GetStatus", new { id = id, fields = fields }),
                  "self",
                  "GET"));
            }

            links.Add(
              new LinkDto(_urlHelper.Link("DeleteStatus", new { id = id }),
              "delete_status",
              "DELETE"));

            links.Add(
              new LinkDto(_urlHelper.Link("CreateBookForStatus", new { statusId = id }),
              "create_book_for_status",
              "POST"));

            links.Add(
               new LinkDto(_urlHelper.Link("GetBooksForStatus", new { statusId = id }),
               "books",
               "GET"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForStatuses(
            StatusesResourceParameters statusesResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateStatusesResourceUri(statusesResourceParameters,
               ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateStatusesResourceUri(statusesResourceParameters,
                  ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateStatusesResourceUri(statusesResourceParameters,
                    ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }

        [HttpOptions]
        public IActionResult GetStatusesOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }

        private void SetItemHistoryData(StatusForUpdationDto model, MstStatus modelRepo)
        {
            model.CreatedOn = modelRepo.CreatedOn;
            model.UpdatedOn = DateTime.Now;
        }
    }
}