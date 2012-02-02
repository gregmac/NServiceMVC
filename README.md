NServiceMVC
===========

NServiceMVC is currently in active development, and is very much alpha-quality software at this point (in fact, it may not even work yet).

The idea behind it is to enable the creation of web services (REST to start with, then SOAP later) using the same controller syntax and setup as ASP.NET MVC 3.

Features
--------

 * Built on MVC 3
 * Automatically handles decoding of posted data (from JSON, XML, etc) and automatically encodes result as JSON/XML/XHTML/etc depending on Accept headers or `?format=` URL parameter
 * Provides (optional) automatic metadata (help) generation for all API paths and model types
 * Attributes are used to configure paths (currently the metadata provider requires this, but the actual API will actually work without attributes, using standard MVC3 routing)
 * Lightweight. The framework gives you just enough to build a REST service; DI containers, authentication, security, caching, logging, etc are up to you to decide and use what you need (or are already using). There are many solutions to all of these problems in the MVC world aleady, so this framework just builds on top.



Show me the code! 
-----------------

An example controller implementation 

```csharp
namespace NServiceMVC.Examples.ComplexApp.Controllers
{
    public class UsersController : NServiceMVC.ServiceController
    {
        [GET("users")] // GET /users
        [Description("List all available users")]
        public List<Models.User> Index()
        {
			return MyServiceLayer.Users.LoadAll();
        }

        [POST("users")] // POST /users
        [Description("Create a new user")]
        public Models.User CreateUser(Models.User user)
        {
            return MyServiceLayer.Users.Create(user);
        }

        [GET("users/{userId}")] // POST /users/123
        [Description("Load a user")]
        public Models.User Detail(int userId)
        {
            return MyServiceLayer.Users.LoadById(userId);
        }

        [PUT("users/{userId}")]  // PUT /users/123
        [Description("Edit an existing user")]
        public bool EditUser(Guid userId, Models.User user)
        {
            user.userId = userId; 
            return MyServiceLayer.Users.Update(user);
        }

        [DELETE("users/{userId}")]  // DELETE /users/123
        [Description("Delete an existing user")]
        public bool EditUser(Guid userId)
        {
            return MyServiceLayer.Users.Delete(userId);
        }
    }
}
```

WebActivator is used to handle startup, but literally the only other code in the MVC project to make the above work is a call to `NServiceMVC.Initialize();`. If you want to enable metadata, there is a small amount of configuration (some of this may become automatic/defaults):

```csharp
NServiceMVC.Initialize(config =>
{
	config.RegisterControllerAssembly(Assembly.GetExecutingAssembly()); 
	config.RegisterModelAssembly(Assembly.GetExecutingAssembly(), namespace:"NServiceMVC.Examples.Models"); 
	config.Metadata("metadata"); // serves metadata at /metadata 
});
```

Motivation
----------

I built this after spending a long time trying to find a way to build REST web services that seemed natural to me, and handled all the framework-type stuff (format encoding) itself. 

With that in mind, I have looked at many other projects:
 
### WCF

I have previously been involved with a commercial product that used WCF to provide combined XML, JSON and SOAP APIs using one codebase, and while it (eventually) worked, it was nothing but problems and getting it configured simply feels like fighting with the framework, not to mention every deployment required some battling with WCF to get it working in the environment. 

I could go in-depth, but suffice to say that WCF does not appeal to me from very many aspects.

### [ServiceStack](http://www.servicestack.net)

This is a great project, and has a lot of the features I was looking for. It's also well maintained and has an active community. It definitely gave me a lot of inspiration, and I think Demis is doing a great job with it.

The underlying philosophy of ServiceStack is that REST is about performing actions on resources, with I agree with. However, the implementation is that the URLs get mapped to resources (DTO's), which in turn have a service implemented as `RestService<MyModel>`, which ServiceStack then uses reflection to find.

My first issue with this is just in the convoluted nature of actually working with it. It takes an extra mental step to map the URLs I want to the code that will run them. I think this may just be me personally, and that the idea behind ServiceStack is the URLs are inconsequential, but every time I try to build something with it I find I get very irritated by this.

The second issue is that you either end up having a LOT of DTOs for your requests, or your DTOs are full of unused members. I'll put some code examples here eventually but basically if you have two different services that both take a userId as a parameter (for example, a service that returns the details about a user, and a service that returns a list of comments by that user), you need to have two different DTOs. The preferred way of ServiceStack seems to be to use the User DTO for the CRUD operations (which means in your GET /user/id method, you just have to ignore all the other properties in the User DTO as they don't get populated, even though they're passed). The other way requires writing two DTOs that are identical (with just a userId property) but different names. It all seems like a lot of ceremony just to get a simple service working.

I also take issue with the implementation of SOAP. Yes, it's possible to service SOAP but it only works with POST requests. This means if you want to make your entire API available using SOAP, all your operations have to be POST, and you're no longer RESTful. 

### [Resources over MVC](http://rom.codeplex.com/)

I think this project is mostly unknown, but this actually is a great project towards what I want to achieve. I basically started building services on this, and adding more and more stuff, before I decided I should somehow incorperate it into the framework. 

As I got doing this, I realized there were some ways to simplify things, and that my project would actually be a very major refactor at the very least, so for now at least, I started working on NServiceMVC. 

The biggest difference is really that my services have a strongly-typed return value (istead of `ActionResult`) and that a base controller is providing a hook to do formatting instead of using global filters to intercept the model before it's passed to the view engine. The downside is that you have to inherit from NServiceMVC.ServiceController, but the upside is you get strong typing and the framework code is much simpler.

Acknowledgements
----------------

Just to acknowledge some of the major sources of code used to get started:

 * [Resources over MVC](http://rom.codeplex.com/) for model binding and format rendering code 
 * [AttributeRouting](https://github.com/mccalltd/AttributeRouting) for ability to specify HTTP verbs and URLs using atributes
 * [ServiceStack](http://www.servicestack.net) for the XHTML versions of model output
 * [WebActivator](https://bitbucket.org/davidebbo/webactivator) used for startup code

Special thanks to [JetBrains](http://www.jetbrains.com/) for providing a [TeamCity Continuous Integration server](http://www.jetbrains.com/teamcity) via [CodeBetter.com](http://codebetter.com/). The builds are available [here](http://teamcity.codebetter.com/project.html?projectId=project182).
 
Future Direction
----------------

I have not started work on this yet, but I am thinking of building a shim so that your `GET /users/id` REST service becomes `POST /SOAP/GET/users` (with id as a parameter) if exposed as SOAP.

Lots more can be done with metadata. The beginnings of an auto-generated AJAX test client are there, this can be built out much more deeply. Schema generation (eg WSDL) is also an obvious place to go. 
 
What I do not want to get into is baking-in any cross-cutting concerns (security, logging, caching, etc), and definitely not in the core library. Right now I don't see a need as effectively the service controllers act exactly like normal MVC controllers, and so any existing libraries or code compatible with MVC will work with services implemented on NServiceMVC. 

