Agri-Energy Connect

Agri-Energy Connect is a web-based application designed to streamline agricultural product management by connecting farmers and employees. The platform offers efficient product and farmer profile management with secure, role-based access.



Purpose

Agri-Energy Connect is designed to bridge the gap between farmers and agricultural employees by providing a centralized platform for managing agricultural products and farmer profiles. The application simplifies the process of tracking production data, categorizing products, and maintaining farmer information, helping to increase operational efficiency and improve communication. By offering role-based access, the system ensures that both farmers and employees can securely manage and monitor relevant data within their scope of work. Ultimately, Agri-Energy Connect aims to empower users to make informed decisions that support sustainable agricultural productivity and streamlined workflows.



Features

Farmer Portal
Add, view, and manage agricultural products
Product categorization and production date tracking
Personalized dashboard for farmers

Employee Dashboard
Manage farmer profiles (add, view, update)
Monitor agricultural production comprehensively
Filter products by farmer profile, product category, and date range

Security
Role-based authentication and authorization using Identity Framework
Separate access controls for Farmers and Employees



Technology Stack

Backend:
ASP.NET Core MVC
Entity Framework Core
Identity Framework (Authentication & Authorization)

Frontend:
Razor Pages

Database:
Visual Studio LocalDB (pre-populated database included)



Installation & Setup

Prerequisites

.NET 8 SDK
Visual Studio 2022 or later (with LocalDB installed, usually bundled)

Steps
1. Clone the repository: https://github.com/ST10260322/Agri-EnergyConnect.git
2. Open the solution in Visual Studio 2022
3. Ensure the LocalDB .mdf database file is included in the project folder
4. Check the connection string in appsettings.json points to your LocalDB instance.
5. Run the application
6. The database will be automatically attached to LocalDB on first run if it’s not already attached
   


Default Accounts:

1. Farmer:
      Username: Messi@gmail.com
      Password: Messi123

2. Employee
      Username: PepGuardiola@gamil.com
      Password: Pep123Pep123



Usage Instructions

For Farmers:
1. Register: Farmers can easily create their own accounts directly from the home page by providing basic information.
2. Manage Products: Once logged in, farmers can add new agricultural products, specifying categories and production dates to keep detailed records.
3. Dashboard: Farmers have access to a personalized dashboard where they can view and update their products at any time.

For Employees:
1. Login: Employees use their credentials to access a dedicated dashboard with extended privileges.
2. Farmer Management: Employees can add new farmer profiles or edit existing ones, enabling them to maintain an up-to-date database of farmers.
3. Product Monitoring: Employees can view all agricultural products across farmers, using filters to sort by farmer, product category, or production date range to efficiently monitor     production.
4. Data Oversight: The dashboard provides employees with comprehensive insight into agricultural activities, helping them support and coordinate farming operations effectively.

General Tips:
1. Use the navigation menus to switch between dashboards and functions smoothly.
2. Take advantage of filtering options in the employee dashboard to quickly locate specific farmers or products.
3. Ensure secure logout after completing your session to protect account access.



Note:
* Farmers can register from the home page.
* However, based on rubric requirements, employees also have the ability to create farmer profiles from within the system.
* This dual registration functionality ensures flexibility and demonstrates the full scope of the application's features.


Additional Notes

* The project uses MVC architecture with Entity Framework Core and Razor Pages.
* The LocalDB database is pre-populated and included in the project folder for ease of setup.
* Tested and verified on Windows 10 and 11 environments.
* Uses .NET 8 SDK for compatibility and stability.
* All code modifications and rubric-related updates are documented in the POE (Portfolio of Evidence) document.
* The project repository is publicly available on GitHub.



References:
https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-9.0&tabs=visual-studio
https://www.c-sharpcorner.com/article/how-to-create-asp-net-core-mvc-application/
https://stackoverflow.com/questions/69706819/redirect-login-user-or-admin-to-specific-page-or-view-depending-upon-role-in-asp
https://learn.microsoft.com/en-us/visualstudio/data-tools/add-new-connections?view=vs-2022
https://stackoverflow.com/questions/59444186/admin-and-user-login-in-mvc
https://stackoverflow.com/questions/68987850/how-to-only-display-items-created-by-the-current-logged-in-user-only-in-asp-net


   
   
