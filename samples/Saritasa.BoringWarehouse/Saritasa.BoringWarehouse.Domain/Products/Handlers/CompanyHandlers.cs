﻿namespace Saritasa.BoringWarehouse.Domain.Products.Handlers
{
    using System.Linq;

    using Tools.Domain.Exceptions;

    using Commands;
    using Entities;
    using Users.Entities;
    using Saritasa.Tools.Messages.Abstractions.Commands;

    /// <summary>
    /// Company handlers.
    /// </summary>
    [CommandHandlers]
    public class CompanyHandlers
    {
        /// <summary>
        /// Handle create command.
        /// </summary>
        public void HandleCreate(CreateCompanyCommand command, IAppUnitOfWorkFactory uowFactory)
        {
            using (IAppUnitOfWork uow = uowFactory.Create())
            {
                if (uow.Companies.Any(c => c.Name == command.Name.Trim()))
                {
                    throw new DomainException("The company with the same name already exists.");
                }
                User creator = uow.Users.FirstOrDefault(u => u.Id == command.CreatedByUserId);
                if (creator == null)
                {
                    throw new DomainException("Cannot find creator.");
                }
                var company = new Company
                {
                    Name = command.Name,
                    CreatedBy = creator
                };
                uow.CompanyRepository.Add(company);
                uow.SaveChanges();
                command.CompanyId = company.Id;
            }
        }

        /// <summary>
        /// Handle update command.
        /// </summary>
        public void HandleUpdate(UpdateCompanyCommand command, IAppUnitOfWorkFactory uowFactory)
        {
            using (IAppUnitOfWork uow = uowFactory.Create())
            {
                if (uow.Companies.Any(c => c.Name == command.Name.Trim() && c.Id != command.CompanyId))
                {
                    throw new DomainException("The company with the same name already exists.");
                }
                Company company = uow.CompanyRepository.Get(command.CompanyId);
                company.Name = command.Name;
                uow.SaveChanges();
            }
        }

        /// <summary>
        /// Handle delete command.
        /// </summary>
        public void HandleDelete(DeleteCompanyCommand command, IAppUnitOfWorkFactory uowFactory)
        {
            using (IAppUnitOfWork uow = uowFactory.Create())
            {
                Company company = uow.CompanyRepository.Get(command.CompanyId);
                uow.CompanyRepository.Remove(company);
                uow.SaveChanges();
            }
        }
    }
}
