﻿using System;
using Saritasa.Tools.Messages.Abstractions;
using Saritasa.Tools.Domain.Exceptions;
using Saritasa.Tools.Messages.Abstractions.Commands;
using ZergRushCo.Todosya.Domain.TaskContext.Commands;

namespace ZergRushCo.Todosya.Domain.TaskContext.Handlers
{
    /// <summary>
    /// Tasks related handlers.
    /// </summary>
    [CommandHandlers]
    public class TaskHandlers
    {
        /// <summary>
        /// Handles task create.
        /// </summary>
        /// <param name="command">Command.</param>
        /// <param name="uowFactory">Application unit of work factory.</param>
        public void HandleCreateTask(CreateTaskCommand command, IAppUnitOfWorkFactory uowFactory)
        {
            using (var uow = uowFactory.Create())
            {
                var project = uow.ProjectRepository.Get(command.ProjectId);
                if (project != null && project.User.Id != command.UserId)
                {
                    throw new DomainException("User does not own the project.");
                }

                var user = uow.UserRepository.Get(command.UserId);

                var task = new Entities.Task()
                {
                    DueDate = DateTime.MaxValue,
                    IsDone = command.IsDone,
                    Text = command.Text.Trim(),
                    Project = project,
                    User = user,
                };
                uow.TaskRepository.Add(task);
                uow.SaveChanges();
                command.TaskId = task.Id;
            }
        }

        /// <summary>
        /// Handles task update.
        /// </summary>
        /// <param name="command">Command.</param>
        /// <param name="uowFactory">Application unit of work factory.</param>
        public void HandleUpdateTask(UpdateTaskCommand command, IAppUnitOfWorkFactory uowFactory)
        {
            using (var uow = uowFactory.Create())
            {
                var project = uow.ProjectRepository.Get(command.ProjectId);
                if (project != null && project.User.Id != command.UserId)
                {
                    throw new DomainException("User does not own the project.");
                }

                var dbtask = uow.TaskRepository.Get(command.Id);
                if (dbtask.User.Id != command.UserId)
                {
                    throw new DomainException("You cannot update task for another user.");
                }

                dbtask.DueDate = DateTime.MaxValue;
                dbtask.IsDone = command.IsDone;
                dbtask.Text = command.Text.Trim();
                dbtask.Project = project;
                uow.SaveChanges();
            }
        }

        /// <summary>
        /// Handles task remove.
        /// </summary>
        /// <param name="command">Command.</param>
        /// <param name="uowFactory">Application unit of work.</param>
        public void HandleRemoveTask(RemoveTaskCommand command, IAppUnitOfWorkFactory uowFactory)
        {
            using (var uow = uowFactory.Create())
            {
                var dbtask = uow.TaskRepository.Get(command.TaskId);
                if (dbtask.User.Id != command.UserId)
                {
                    throw new DomainException("You cannot remove task for another user.");
                }

                uow.TaskRepository.Remove(dbtask);
                uow.SaveChanges();
            }
        }

        /// <summary>
        /// Handles task check/uncheck (done/not done).
        /// </summary>
        /// <param name="command">Command.</param>
        /// <param name="uowFactory">Application unit of work factory.</param>
        public void HandleCheckTask(CheckTaskCommand command, IAppUnitOfWorkFactory uowFactory)
        {
            using (var uow = uowFactory.Create())
            {
                var dbtask = uow.TaskRepository.Get(command.TaskId);
                if (dbtask.User.Id != command.UserId)
                {
                    throw new DomainException("You cannot check/uncheck task for another user.");
                }

                dbtask.IsDone = command.IsDone;
                uow.SaveChanges();
            }
        }
    }
}
