NServiceMVC
===========

NServiceMVC is currently in active development, and is currently being built in conjunction with a piece of commercial software. 

Current releaes is 1.0.1 and can be obtained via NuGet. See the [getting started guide](https://github.com/gregmac/NServiceMVC/wiki/GettingStarted) for installation and usage instructions.

The idea behind it is to enable the creation of web services (REST to start with, then SOAP later) using the same controller syntax and setup as ASP.NET MVC 3.

This is actually very similar to [ASP.NET 4 Web API](http://www.asp.net/whitepapers/mvc4-release-notes#_Toc317096197), but of course NServiceMVC works on MVC 3.

> I started building this before knowing that Web API existed, and they surprised me with a release before I even widely published this. It's unclear exactly what will happen with NServiceMVC, but I am continuing to work on it at least until the final release of ASP.NET 4. I suspect at the least NServiceMVC will continue to exist to supplement Web API, and possibly will continue to exist as-is to offer an alternative.

Features
--------

 * Built on MVC 3
 * Automatically handles decoding of posted data (from JSON, XML, etc) and automatically encodes result as JSON/XML/XHTML/etc depending on Accept headers or `?format=` URL parameter
 * Provides (optional) automatic metadata (help) generation for all API paths and model types
 * Attributes are used to configure paths (currently the metadata provider requires this, but the actual API will actually work without attributes, using standard MVC3 routing)
 * Lightweight. The framework gives you just enough to build a REST service; DI containers, authentication, authorization, caching, logging, etc are up to you to decide and use what you need (or are already using). There are many solutions to all of these problems in the MVC world aleady, it is not the goal of this framework to either dictate or re-implement such functionality.

![Todomvc Metadata Page](http://i.imgur.com/qzkOw.png)

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

        [GET("users/{userId}")] // GET /users/123
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

Todomvc
-------

Also check out the [NServiceMVC + Todomvc Example](https://github.com/gregmac/NServiceMVC.Examples.Todomvc).

Acknowledgements
----------------

Just to acknowledge some of the major sources of code used to get started:

 * [Resources over MVC](http://rom.codeplex.com/) for model binding and format rendering code, and the initial building blocks of using MVC to build REST services 
 * [AttributeRouting](https://github.com/mccalltd/AttributeRouting) for ability to specify HTTP verbs and URLs using atributes
 * [ServiceStack](http://www.servicestack.net) for the XHTML versions of model output, and some inspiration in terms of service frameworks
 * [WebActivator](https://bitbucket.org/davidebbo/webactivator) used for startup code

Special thanks to [JetBrains](http://www.jetbrains.com/) for providing a [TeamCity Continuous Integration server](http://www.jetbrains.com/teamcity) via [CodeBetter.com](http://codebetter.com/). The builds are available [here](http://teamcity.codebetter.com/project.html?projectId=project182).
 
Future Direction
----------------

The biggest challenge is the release of ASP.NET 4 Web API. It is likely at the least that this project will build on top of new features introduced there, and either supplement Web API with its metadata generation, SOAP/XML-RPC support (if implemented), or simply offer an alternative implementation.

I have not started work on this yet, but I am thinking of building a shim so that your `GET /users/id` REST service becomes something like `POST /SOAP/GET_users` (with id as a parameter) if exposed as SOAP (since SOAP has totally different semantics and limitations than REST). Same thing could be done for XML-RPC.

Lots more can be done with metadata. The beginnings of an auto-generated AJAX test client are there, this can be built out much more deeply. Schema generation (eg WSDL) is also an obvious place to go. 
 
What I do not want to get into is baking-in any cross-cutting concerns (security, logging, caching, etc), and definitely not in the core library. Right now I don't see a need as effectively the service controllers act exactly like normal MVC controllers, and so any existing libraries or code compatible with MVC will work with services implemented on NServiceMVC. 
