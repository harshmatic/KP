﻿using ESPL.KP.Entities;
using ESPL.KP.Helpers;
using System;
using System.Collections.Generic;
using ESPL.KP.Entities.Core;
using ESPL.KP.Helpers.Core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ESPL.KP.Services
{
    public interface ILibraryRepository
    {
        PagedList<Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters);
        Author GetAuthor(Guid authorId);
        IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds);
        void AddAuthor(Author author);
        void DeleteAuthor(Author author);
        void UpdateAuthor(Author author);
        bool AuthorExists(Guid authorId);
        IEnumerable<Book> GetBooksForAuthor(Guid authorId);
        Book GetBookForAuthor(Guid authorId, Guid bookId);
        void AddBookForAuthor(Guid authorId, Book book);
        void UpdateBookForAuthor(Book book);
        void DeleteBook(Book book);
        bool Save();

        #region AppModule

        PagedList<AppModule> GetAppModules(AppModulesResourceParameters appModuleResourceParameters);
        AppModule GetAppModule(Guid appModuleId);
        IEnumerable<AppModule> GetAppModules(IEnumerable<Guid> appModuleIds);
        void AddAppModule(AppModule appModule);
        void DeleteAppModule(AppModule appModule);
        void UpdateAppModule(AppModule appModule);
        bool AppModuleExists(Guid appModuleId);

        bool AppModuleExists(string appModuleName);

        #endregion AppModule

        #region ESPLUser

        PagedList<ESPLUser> GetESPLUsers(ESPLUsersResourceParameters esplUserResourceParameters);
        ESPLUser GetESPLUser(Guid esplUserId);
        IEnumerable<ESPLUser> GetESPLUsers(IEnumerable<Guid> esplUserIds);
        void AddESPLUser(ESPLUser esplUser);
        void DeleteESPLUser(ESPLUser esplUser);
        void UpdateESPLUser(ESPLUser esplUser);
        bool ESPLUserExists(Guid esplUserId);

        #endregion ESPLUser


        #region ESPLRole

        PagedList<IdentityRole> GetESPLRoles(ESPLRolesResourceParameters esplRoleResourceParameters);
        IdentityRole GetESPLRole(Guid esplRoleId);
        IEnumerable<IdentityRole> GetESPLRoles(IEnumerable<Guid> esplRoleIds);
        void AddESPLRole(IdentityRole esplRole);
        void DeleteESPLRole(IdentityRole esplRole);
        void UpdateESPLRole(IdentityRole esplRole);
        bool ESPLRoleExists(Guid esplRoleId);

        #endregion ESPLRole
    }
}
