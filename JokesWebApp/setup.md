# .Net setup.


## Step 1- Create new dot net core application

Using the following command, we can create new .NET Core Application. It can be either Web or desktop. The "dotnet" is a general driver to run the commands on CLI (Command Line Interface). Visual studio code works well with other tools like command-line tool. We can also run "dotnet" command, using command prompt.

```js
dotnet new
```

When we run the command, shown above, it creates an empty project. The empty project mainly contains two files, which are program.cs and project.json.

Using Visual Studio:

Basic Joke App.
================

Create a new project, then select web app with MVC. Select authentication (single user) included. This will generate all the files and folders necessary to run. Clicking the play button will show the app template in the browser.
The url in the browser will show the locaL host port. (https://localhost:7088/)
You should see a number of pages that are already setup.

App Name - Home - Privacy - Register - Login.

So where are these coming from?

Inside the Views/Home folder you will find the index.cshtml and the privacy.cshtml. (C Sharp HTML)
We can modify these files to display some text relevant to our app.

Our app will be employing the Model View Controller approach.
Model - Classes (objects)
View - Web Page (Razor HTML)
Controller - Connects models, business logic and web pages.

The MVC design pattern helps enforce seperation of concerns to help avoid mixing presentational logic, business logic and data access logic together.

For our jokes app, each joke will be an instance of the joke class and will have a question, answer and Id.
We will require a view to manage the display of data.
Our controller will handle page events and navigation between pages.


## Model
Lets start by creating our joke class.
Right click on models and create a new class named Joke.
Inside the class type 'prop' and hit tab twice to produce some pre-written code describing our first class property. Next change the values to suit.
In C# we create getters and setters by putting get; and set; inside the brackets
Create two more properties for JokeQuestion and JokeAnswer.
The '=default!' portion indicates that the values are nullable.
```js
using System;
namespace JokesWebApp.Models
{
    public class Joke{
        public int Id { get; set; }
        public string JokeQuestion { get; set; }
        public string JokeAnswer { get; set; }

    }
}
```

Next we need a constructor. Typing
```js
ctor
```
will create the basic constructor. Leave the constructor empty as this will be used for creating instances from the db much like Spring uses a default constructor.
```js
using System;
namespace JokesWebApp.Models
{
    public class Joke{
        public int Id { get; set; }
        public string JokeQuestion { get; set; }
        public string JokeAnswer { get; set; }

        public Joke()
        {

        }

    }
}
```

## Controller
Lets turn our attention to the controller.
Right click on the controller folder and select 'new scaffolding'.
Click 'MVC Controller with views, using entity frameworks'.
This is going to generate quite alot of code for us. Next, select the model we want to create the controller for. In this case Joke.
IN the field for 'DbContext class to use' enter
```js
JokesWebApp.Data.ApplicationDbContext
```
Ensure that the following are selected;
Generate Views
Reference Script Libraries
Use a Layout Page.

In the Controller Name field, enter 'JokesController'
Click Finish.

This should create a JokeController file as well as views for each of the CRUD functions.
If we start the app and go to /Joke we will see 'A Database operation failed'
So the page exists but the database is currently empty. Next, lets remedy that.

## Database Migrations
First off, Code for mac does not include a package manager console. We can get around this by installing ef (Entity Framework. Entity is Microsoft's Object Relational Mapper);
```js
dotnet tool install --global dotnet-ef
export PATH="$PATH:/Users/'your user folder'/.dotnet/tools"
dotnet restore
dotnet ef
```
Next, run;
```js
dotnet ef migrations add initial
```
If successfull, go to the project and in the Data/Migrations folder you should see a new file ending "..._initial.cs".
In this file are the instructions to create the Joke table.
To make this take effect we enter;
```js
dotnet ef database update
```

We can check this has been successfull by looking at the tables in sqlite.
From the propject root directory;
```js
sqlite3
.open app.db
.databases
.tables
```
You should see a Joke table.

If we restart the app and go to /joke we should no longer get the error. Instead we have an index page which is currently unpopulated.
If you click create, you should now be able to add jokes, then edit, delete, etc.

## Additional Features.
We now have a fully functioning CRUD app. we can add some features. For example,
1. Add items to Navbar
2. Search function..
3. Hide the joke answer.
4. Limit create to logged in users.


1. Add items to navbar.
========================
Go to the Shared folder and open the _Layout.cshtml file.
Here you should see the Navbar.
```js
<li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
</li>
```

Lets add our link;
```js
<li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-controller="Joke" asp-action="Index">Jokes</a>
</li>
```
The 'asp-action' can be determined by looking at the controller.
```js
// GET: Joke
public async Task<IActionResult> Index()
    {
        return _context.Joke != null ? 
        View(await _context.Joke.ToListAsync()) :
        Problem("Entity set 'ApplicationDbContext.Joke'  is null.");
    }
```
Restarting the app should make this link appear and it should lead us to the all jokes page.



2. Search Function
===================

First off, lets create another nav link
```js
<li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-controller="Joke" asp-action="ShowSearchForm">Search</a>
</li>
```

The asp-action "ShowSearchForm" currently doesn't exist, so lets go to the controller and add the following;
```js
// GET: Joke/ShowSearchForm
public async Task<IActionResult> ShowSearchForm()
    {
        return _context.Joke != null ?
            View() :
            Problem("Entity set 'ApplicationDbContext.Joke'  is null.");
    }
```

We need a view to send back when this function is called.
Right click on Views/Joke and select add new scaffolding. Select Razor page.
Name: ShowSearchForm
Also, click 'create partial view' to make the new view part of the main template.
As a starting point, copy the contents of the create view into the new view.

Lets remove the first input field as we don't need it. Also, we'll change the value of the submit button to "Search".
Remove the @model line since we are not creating a new joke.
Change the title to "Search For A Joke".
Remove the validation code from the bottom of the page.
Change the label for="SearchPhrase"
Change the input name="SearchPhrase"
Remove the input validation.
Change the form action to "ShowSearchResults" (We'll write this function shortly.)

````html
<h4>Search For A Joke</h4>
<hr />
<div class="row">
    <div class="col-md-4">
        <form asp-action="ShowSearchResults">
            <div class="form-group">
                <label for="SearchPhrase" class="control-label"></label>
                <input name="SearchPhrase" class="form-control" />
            </div>
            <div class="form-group">
                <input type="submit" value="Create" class="btn btn-primary" />
            </div>
        </form>
    </div>
</div>

<div>
    <a asp-action="Index">Back to List</a>
</div>
````

Due to the form asp-action="ShowSearchResults", submitting our form will take us to /joke/ShowSearchResults.
We need to go to the controller and add the method.
We can check that we are receiving the user's input by simply returning a string.
```js
// Post: Joke/ShowSearchResults
    public String ShowSearchResults(String SearchPhrase)
    {
        return "You entered " + SearchPhrase;
    }
```

Now that we have the search term, we can use it to generate a filtered output;
```js
// Post: Joke/ShowSearchResults
    public async Task<IActionResult> ShowSearchResults(String SearchPhrase)
    {
        return View("Index", await _context.Joke.Where( j => j.JokeQuestion.Contains(SearchPhrase)).ToListAsync());
    }
```
The additional argument on the view method says that we want to return an index view.
This index view will be populated with the results of the second argument. (Filter all jokes where the joke.question contains the SearchPhrase).


3. Hide the joke answer column.
===============================

If we look in Views/Joke/index we can see that the html has a table with 2 headers. One for joke question and one for joke answer.
Lets remove the header for the joke answer. This will remove the joke answer column from the table.
```js
    <thead>
        <tr>
            <th>
                @Html.DisplayNameFor(model => model.JokeQuestion)
            </th>
            <th></th>
        </tr>
    </thead>
```

Next remove the joke answer rows from the table.
```js
        <tr>
            <td>
                @Html.DisplayFor(modelItem => item.JokeQuestion)
            </td>
            <td>
                <a asp-action="Edit" asp-route-id="@item.Id">Edit</a> |
                <a asp-action="Details" asp-route-id="@item.Id">Details</a> |
                <a asp-action="Delete" asp-route-id="@item.Id">Delete</a>
            </td>
        </tr>
```

Now the app should show only the joke questions. The user needs to click on details to see the answer/punch line.


4. Limit create to logged in users.
====================================

In the Joke controller we're going to add a decorator to our create function.
```js
[Authorize] //ADDED
// GET: Joke/Create
public IActionResult Create()
   {
    return View();
    }
```

In order to use this decorator we will need to import a library from using Microsoft.AspNetCore.Authorization;
Once done, clicking create in the app should point us to the login page. Only once we are logged in will be able to create new jokes.


## Note on CSS.
Making css changes is done by editing the wwroot/css/site.css.
Anything defined in here will overwrite the bootsrap css. There is also a js file for scripting css using js.

















