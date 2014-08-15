cms-app
=======

This repository contains a subset of the source code for a Content Management System (CMS) application created for a school assignment. It was written in C# using the ASP.NET MVC framework and Razor templating, and the full application was deployed via Microsoft Azure.

The finished application is located at [cms-app-nyu.azurewebsites.net](http://cms-app-nyu.azurewebsites.net/). It was created by a three person team including me and two others. This repository contains only the source code files that were written solely by me. It does not contain the source code files that were written by my teammates.

###Application Overview

This application manages educational content. Instructors can create courses and post materials to courses. Students can enroll in courses and view course materials. The application admin can edit users & courses, deactivate & activate users and courses, etc. Please feel free to register as a new user (you can use a fake name & email address) and explore the application.

###My Contributions to the Application

I was responsible for the portions of the project that manager users (including creating, updating, displaying users) and the survey portion (students rate course via survey; instructors and admins view results).

First, I made the .NET Identity system work with our own User table in our Entity Framework (EF) database. I modified the auto-generated AccountController.Register method & corresponding view to gather the necessary input and create a User instance in our EF database corresponding to the Identity user instance created in the AspNetUsers table. I also set up the Web.config file to make sure both the Identity system and the EF system were using the same database, which we created in Azure. This way we didn't have to struggle with deploying our database(s) since everything was in one database that was in the cloud all along.

Next I set up Roles using the Identity system so that we could control access to certain views & controller actions. I wrote code in the UserController and StudentController files that makes sure the User.Role property in the User table of our EF database and the AspNetUserRoles table from the Identity system are always consistent with one another. If a user's role is updated in EF via one of the Edit views, I made sure it is also updated in Identity.

We have three user roles: admin, instructor, and student. I created the user and student controllers, and updated the teacher controller which was initially create by another teammate. I also created the corresponding user and student views, and updated the teacher views which were initially started by that same teammate. These controllers and views allow users of the app to view the existing users saved in the database. The user controller allows a user of the admin role to view/edit/deactivate/activate all existing users. The student controller allows admins to do all of those functions specifically for student users only. The teacher controller allows admins to to all of those functions for teacher users only. Finally, the student and teacher controller also allow non-admin users to view existing student and teacher users, respectively.

In the process of creating the controllers and views described in the previous paragraph, I also created a variety of View Models. These can be found in the CMS_App/Models folder. These allowed me to pass all necessary data to and from the user/student/teacher views, without the views needing to access or be aware of the data layer (which is named BizLayer).

The user/student/teacher controllers and related view models access the database using BizLayer/ManagerClasses/UserManager, which I wrote.

Lastly, I created the survey functionality. This allows users to rate a course via survey questions, and instructors and admins can view aggregate results. I wrote the SurveyManger.cs file, the SurveyController.cs file, and the corresponding view models and views. If a student re-takes a survey, the survey is pre-populated with the prior response, and that prior response is overwritten by the newly submitted response. This prevents a student from saving numerous survey responses in the database and potentially skewing the survey results.
