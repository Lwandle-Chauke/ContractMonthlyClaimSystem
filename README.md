# **Contract Monthly Claim System (CMCS)**

## Overview

The **Contract Monthly Claim System (CMCS)** is a web-based application developed using **C#** and **.NET Core MVC** framework. The system allows lecturers to submit claims for hours worked, and Programme Coordinators to manage and approve/reject those claims. Academic Managers can also track and view the overall status of submitted claims. 

The application includes:
- **Lecturer Dashboard**: Submit and track claims.
- **Programme Coordinator Dashboard**: Review and approve/reject claims.
- **Role-Based Access**: Different users (Lecturers, Coordinators, Managers) have access to different functionality.
- **Document Upload**: Supporting documents can be uploaded with each claim.
- **Claim Status Tracker**: Track the status of submitted claims.

## Features

1. **User Registration**:
   - Roles: Lecturers, Programme Coordinators, and Academic Managers.
   - Form Validation: Ensures data integrity for each user’s registration details.

2. **Login & Authentication**:
   - Role-based redirection after login.

3. **Claim Submission (Lecturer)**:
   - Submit claims for hours worked with supporting documentation.
   - Track claim status (Pending, Approved, Rejected).

4. **Claim Management (Coordinator)**:
   - View submitted claims.
   - Accept or reject claims.

5. **Claim Tracking**:
   - Track all submitted claims for Lecturers and Managers.
   - Coordinators can filter claims and update statuses.

## Project Structure

```bash
PROGPart2/
├── Controllers/         # Contains all the MVC controllers like AccountController and ClaimController
├── Models/              # Entity classes and ViewModels for User, Claim, and Role
├── Views/               # Razor views for UI
├── wwwroot/             # Static files like CSS, JS, images
├── Data/                # CMCSContext for database connection and migrations
├── UnitTests/           # Unit tests for different functionalities
└── README.md            # This file
```

## Setup Instructions

### Prerequisites

To run this project, ensure you have the following software installed:

- **.NET Core SDK** (Version 6.0 or later)
- **Visual Studio** (or any IDE that supports .NET Core)
- **SQL Server** (for database storage)
- **Git** (to clone the repository)

### Cloning the Repository

1. Open a terminal window and run the following command to clone the repository:

   ```bash
   git clone https://github.com/YourUsername/PROGPart2.git
   ```

2. Navigate into the project directory:

   ```bash
   cd PROGPart2
   ```

### Database Setup

1. In the `appsettings.json` file, configure your **SQL Server** connection string:

   ```json
   "ConnectionStrings": {
     "CMCSContext": "Server=YOUR_SERVER_NAME;Database=CMCS_DB;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

2. Run the following command to apply migrations and create the database:

   ```bash
   dotnet ef database update
   ```

### Running the Application

1. Open the solution in **Visual Studio** or your preferred IDE.
2. Build the project to restore dependencies.
3. Run the application by pressing `F5` or using the command line:

   ```bash
   dotnet run
   ```

4. The application should now be running on `https://localhost:5001`.

### Unit Tests

The project includes unit tests to verify the functionality of:
- Account registration
- Login
- Claim submission
- Claim tracking and management

To run the unit tests:

1. Open the **Test Explorer** in Visual Studio.
2. Click "Run All Tests" to execute the tests.

Alternatively, you can run the tests via the command line:

```bash
dotnet test
```

## Usage Instructions

### 1. **Register a New User**
   - Navigate to the **Registration** page.
   - Fill out the form with your details, including selecting a role (Lecturer, Coordinator, or Manager).
   - After successful registration, log in to access your dashboard.

### 2. **Submitting a Claim (Lecturer)**
   - After logging in as a lecturer, go to the **Submit Claim** page.
   - Fill in the required details such as hours worked, hourly rate, and upload any supporting documents.
   - Submit the claim and track its status on the **Track Claims** page.

### 3. **Managing Claims (Coordinator)**
   - Coordinators can review all submitted claims from the **Manage Claims** section.
   - Claims can either be **Accepted** or **Rejected**.
   - Once a claim’s status is updated, the lecturer can see the updated status in their **Track Claims** section.

### 4. **Tracking Claims (Lecturer & Managers)**
   - Lecturers can track the status of their submitted claims in the **Track Claims** section.
   - Academic Managers can view the overall status of claims submitted across the organization.

## Technologies Used

- **.NET Core MVC**: For the web framework and application structure.
- **C#**: Backend logic and database interaction.
- **Entity Framework Core**: For database access and management.
- **SQL Server**: For data storage.
- **Razor Pages**: For rendering views and UI.
- **Bootstrap**: For styling and responsive design.
- **Unit Testing**: NUnit or xUnit for testing the functionality of the system.

## Contributions

Contributions are welcome! To contribute:

1. Fork the repository.
2. Create a new feature branch.
3. Make your changes.
4. Submit a pull request for review.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact Information

For any issues or suggestions, feel free to reach out:

- **Project Owner**: Lwandle Chauke 
- **Email**: ST10380788@imconnect.edu.za
