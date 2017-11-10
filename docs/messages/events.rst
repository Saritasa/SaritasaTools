Events
======

Overview
--------

Things happen. Not all of them are interesting, some may be worth recording but don't provoke a reaction. The most interesting ones cause a reaction. Many systems need to react to interesting events only. Events are reactions to something that happens to system. They are executed separately. For example after user creation welcome email should be sent to him. User creation is a command and email sending is an event.

    ::

        User Create ---> (events pipeline) ---> Email Send

Here is how it can be used:

1. Setup pipeline service. Also you need to register it with your DI container since it can be injected to command hanlders.

2. Create event class:

    .. code-block:: c#

        public class UserCreatedEvent
        {
            public User User { get; set; }
        }

3. Prepare event handler, it should be class with attribute ``EventHandlers``, the method name should start with ``Handle`` prefix and first argument should be event class.

    .. code-block:: c#

        [EventHandlers]
        public class UserEventsHandlers
        {
            public void HandleSendEmailOnUserCreate(UserCreatedEvent userCreatedEvent,
                Saritasa.Tools.Emails.IEmailSender<MailMessage> emailSender)
            {
                var message = new MailMessage
                {
                    To = { new MailAddress(userCreatedEvent.User.Email) },
                    Body = $"Thanks for registering to our site!"
                };
                emailSender.SendAsync(message);
            }
        }

Middlewares
-----------

    .. class:: DomainEventLocatorMiddleware

        Uses domain events manager to raise events.

    .. class:: EventHandlerExecutorMiddleware

        Included to default pipeline. Default event executor. It does not process events with Rejected status.

    .. class:: EventHandlerLocatorMiddleware

        Included to default pipeline. Locates command hanlder. See requirements above.

Default Pipeline
----------------

    ::

        EventHandlerLocatorMiddleware ---> EventHandlerResolverMiddleware -> EventHandlerExecutorMiddleware
