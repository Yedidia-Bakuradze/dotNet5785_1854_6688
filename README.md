# Volunteer Management Platform - Mini Project in Windows Systems

Welcome to the Volunteer Management Platform! This project is designed to help organizations manage volunteers and the calls they can assign to themselves. The platform provides a call recommendation feature that updates based on the user's geographical location and allows setting a maximum distance for call assignments.

The project is built using a multi-tier architecture, with each week representing a stage in the development process. Each stage is tagged with a release for easy reference.

### Stages

- [0th Stage: Initialization](#0th-stage-initialization)
- [1st Stage: Data Access Layer](#1st-stage-data-access-layer)
- [2nd Stage: Enhancements and Improvements](#2nd-stage-enhancements-and-improvements)
- [3rd Stage: Adding XML Data Storage](#3rd-stage-adding-xml-data-storage)
- [4th Stage: Business Logic Layer](#4th-stage-business-logic-layer)
- [5th Stage: Presentation Layer (UI)](#5th-stage-presentation-layer-ui)

## 0th Stage: Initialization

The initial stage focuses on setting up the project and learning the basic principles of using Git and GitHub. This stage includes:

- Setting up the project repository
- Understanding version control with Git
- Learning how to collaborate using GitHub

## 1st Stage: Data Access Layer

In this stage, we developed the Data Access Layer (DAL) of the project. This layer is responsible for interacting with the database and includes the following components:

- **DALTest**: Tests for the DAL components to ensure data integrity and correctness.
- **DALFacade**: A facade pattern implementation to provide a simplified interface to the complex DAL operations.
- **DALList**: A list-based implementation for managing data in memory.
- **DO Entities**: Data Objects (DO) that represent the entities in the system.

This stage lays the foundation for the data management aspects of the platform, ensuring that data operations are efficient and reliable.

## 2nd Stage: Enhancements and Improvements

In this stage, we made several enhancements and improvements to the project, including:

- **New Exception Types**: Introduced new exception types for handling specific scenarios, such as items that do not exist and items that have already been added. These exceptions improve error handling and provide more informative error messages.
- **Enumerable Collection for Database**: The database now uses the `Enumerable` collection instead of the `List` collection. This change allows the database to be used for more generic purposes and provides greater flexibility in data management.
- **LINQ Expressions for Read and ReadAll Methods**: The `Read` and `ReadAll` methods now use LINQ expressions instead of `List` methods to retrieve content from the database. This change improves the efficiency and readability of data retrieval operations.

These enhancements improve the overall functionality and maintainability of the platform, making it more robust and versatile.

## 3rd Stage: Adding XML Data Storage

### Background

In this step, we added the capability to store data in XML format. We implemented all the necessary interfaces and properties to support XML data storage. Additionally, we updated the `DALTest` to utilize the XML storage functionality.

### Implementation Details

**XML Data Storage**:

- We introduced a new class `XmlDataStorage` that implements the `IDataStorage` interface.
- This class handles reading from and writing to XML files, ensuring that data is correctly serialized and deserialized.

**Interface Implementation**:

- All relevant interfaces were updated to include methods for XML data handling.
- The `XmlDataStorage` class provides concrete implementations for these methods.

**DALTest Update**:

- The `DALTest` class was modified to use the `XmlDataStorage` class.
- This allows `DALTest` to perform its operations using XML files, ensuring that the new storage mechanism is thoroughly tested.

## 4th Stage: Business Logic Layer

### Background

In this stage, we added a Business Logic (BL) layer to the project, including new Business Objects (BOs). The BL layer is responsible for implementing the core logic of the application, ensuring that data is processed correctly and efficiently.

### Implementation Details

**Business Objects (BOs)**:

We introduced new BO classes, including:

- `BO.Volunteer`
- `BO.VolunteerInList`
- `BO.Call`
- `BO.CallInList`
- `BO.OpenCallInList`
- `BO.ClosedCallInList`
- `BO.Assignment`

These BO classes represent the business entities and are central to the application's logic.

**Logic Layer**:

- **Data Creation and Validation**: Implemented functionality to create new DO entities in the database. The logic layer checks the validity of input values from the user to ensure data integrity.
- **Volunteer Management**:
  - Volunteers can now assign and complete calls.
  - Volunteers are created with strong passwords that are hashed using SHA256 for security. The hashed passwords are stored in the XML files.
- **Address Validation**:
  - Addresses entered during call and volunteer creation are validated for correctness.
  - Coordinates (latitude and longitude) are calculated using Google's Geocoding API based on the validated addresses.
- **Distance Calculation**:
  - The distance between a volunteer and a call is calculated using Google's Distance Matrix API.
  - This ensures accurate distance measurements for assigning calls within a volunteer's maximum distance preference.
- **Call Assignment and Completion**:
  - Implemented logic to allow volunteers to assign themselves to calls.
  - Volunteers can complete calls, and the system updates the call status accordingly.

**Factory Design Pattern**:

- Implemented a Factory design pattern to provide a flexible option for selecting the DAL layer's implementation type (XML or List).
- This design pattern enhances scalability and maintainability by decoupling the BL layer from specific DAL implementations.

This stage enhances the platform by adding a robust business logic layer, improving data validation processes, and integrating real-time geolocation services for a better user experience.

## 5th Stage: Presentation Layer (UI)

### Background

In this stage, we implemented several windows in the Presentation Layer (PL), which is our UI layer. These windows provide a user-friendly interface for managing volunteers and their assignments.

### Implementation Details

**Volunteer List Window**:

- A window where the user can see all volunteers in a list and filter them by their call type.

**Configuration Window**:

- A window where the user can reset and initialize the database, change the risk range, and forward the clock by a second, minute, day, month, or year.

**Volunteer Window**:

- A window that shows all volunteer details and allows the user to update or add a new volunteer.

**Call In Progress Window**:

- A read-only window that displays the values of the call in progress for a specific user.

These windows enhance the user experience by providing a clear and intuitive interface for managing volunteers and their assignments.

## Features

- **Local Database Management**: Allows the user to add new entity records to the local database stored in a simple list structure.
- **DO Entities Description**: A class implementation has been added for every entity that would be involved in the project including the Call, Assignment, and Volunteer entities.
- **Main Program for Testing Purposes**: Provides a program in which the developer can simulate the functionality of the CRUD APIs including adding, updating, removing, and retrieving entity records from the API layer.

## Getting Started

To get started with the project, clone the repository and follow the instructions in the respective stage documentation.

## Contributing

We welcome contributions from the community. Please read our [contributing guidelines](CONTRIBUTING.md) for more information on how to get involved.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.# Volunteer Management Platform - Mini Project in Windows Systems

Welcome to the Volunteer Management Platform! This project is designed to help organizations manage volunteers and the calls they can assign to themselves. The platform provides a call recommendation feature that updates based on the user's geographical location and allows setting a maximum distance for call assignments.

The project is built using a multi-tier architecture, with each week representing a stage in the development process. Each stage is tagged with a release for easy reference.

### Stages

- [0th Stage: Initialization](#0th-stage-initialization)
- [1st Stage: Data Access Layer](#1st-stage-data-access-layer)
- [2nd Stage: Enhancements and Improvements](#2nd-stage-enhancements-and-improvements)
- [3rd Stage: Adding XML Data Storage](#3rd-stage-adding-xml-data-storage)
- [4th Stage: Business Logic Layer](#4th-stage-business-logic-layer)
- [5th Stage: Presentation Layer (UI)](#5th-stage-presentation-layer-ui)
- [6th Stage: Post-Presentation Layer Enhancements](#6th-stage-post-presentation-layer-enhancements)

## 0th Stage: Initialization

The initial stage focuses on setting up the project and learning the basic principles of using Git and GitHub. This stage includes:

- Setting up the project repository
- Understanding version control with Git
- Learning how to collaborate using GitHub

## 1st Stage: Data Access Layer

In this stage, we developed the Data Access Layer (DAL) of the project. This layer is responsible for interacting with the database and includes the following components:

- **DALTest**: Tests for the DAL components to ensure data integrity and correctness.
- **DALFacade**: A facade pattern implementation to provide a simplified interface to the complex DAL operations.
- **DALList**: A list-based implementation for managing data in memory.
- **DO Entities**: Data Objects (DO) that represent the entities in the system.

This stage lays the foundation for the data management aspects of the platform, ensuring that data operations are efficient and reliable.

## 2nd Stage: Enhancements and Improvements

In this stage, we made several enhancements and improvements to the project, including:

- **New Exception Types**: Introduced new exception types for handling specific scenarios, such as items that do not exist and items that have already been added. These exceptions improve error handling and provide more informative error messages.
- **Enumerable Collection for Database**: The database now uses the `Enumerable` collection instead of the `List` collection. This change allows the database to be used for more generic purposes and provides greater flexibility in data management.
- **LINQ Expressions for Read and ReadAll Methods**: The `Read` and `ReadAll` methods now use LINQ expressions instead of `List` methods to retrieve content from the database. This change improves the efficiency and readability of data retrieval operations.

These enhancements improve the overall functionality and maintainability of the platform, making it more robust and versatile.

## 3rd Stage: Adding XML Data Storage

### Background

In this step, we added the capability to store data in XML format. We implemented all the necessary interfaces and properties to support XML data storage. Additionally, we updated the `DALTest` to utilize the XML storage functionality.

### Implementation Details

**XML Data Storage**:

- We introduced a new class `XmlDataStorage` that implements the `IDataStorage` interface.
- This class handles reading from and writing to XML files, ensuring that data is correctly serialized and deserialized.

**Interface Implementation**:

- All relevant interfaces were updated to include methods for XML data handling.
- The `XmlDataStorage` class provides concrete implementations for these methods.

**DALTest Update**:

- The `DALTest` class was modified to use the `XmlDataStorage` class.
- This allows `DALTest` to perform its operations using XML files, ensuring that the new storage mechanism is thoroughly tested.

## 4th Stage: Business Logic Layer

### Background

In this stage, we added a Business Logic (BL) layer to the project, including new Business Objects (BOs). The BL layer is responsible for implementing the core logic of the application, ensuring that data is processed correctly and efficiently.

### Implementation Details

**Business Objects (BOs)**:

We introduced new BO classes, including:

- `BO.Volunteer`
- `BO.VolunteerInList`
- `BO.Call`
- `BO.CallInList`
- `BO.OpenCallInList`
- `BO.ClosedCallInList`
- `BO.Assignment`

These BO classes represent the business entities and are central to the application's logic.

**Logic Layer**:

- **Data Creation and Validation**: Implemented functionality to create new DO entities in the database. The logic layer checks the validity of input values from the user to ensure data integrity.
- **Volunteer Management**:
  - Volunteers can now assign and complete calls.
  - Volunteers are created with strong passwords that are hashed using SHA256 for security. The hashed passwords are stored in the XML files.
- **Address Validation**:
  - Addresses entered during call and volunteer creation are validated for correctness.
  - Coordinates (latitude and longitude) are calculated using Google's Geocoding API based on the validated addresses.
- **Distance Calculation**:
  - The distance between a volunteer and a call is calculated using Google's Distance Matrix API.
  - This ensures accurate distance measurements for assigning calls within a volunteer's maximum distance preference.
- **Call Assignment and Completion**:
  - Implemented logic to allow volunteers to assign themselves to calls.
  - Volunteers can complete calls, and the system updates the call status accordingly.

**Factory Design Pattern**:

- Implemented a Factory design pattern to provide a flexible option for selecting the DAL layer's implementation type (XML or List).
- This design pattern enhances scalability and maintainability by decoupling the BL layer from specific DAL implementations.

This stage enhances the platform by adding a robust business logic layer, improving data validation processes, and integrating real-time geolocation services for a better user experience.

## 5th Stage: Presentation Layer (UI)

### Background

In this stage, we implemented several windows in the Presentation Layer (PL), which is our UI layer. These windows provide a user-friendly interface for managing volunteers and their assignments.

### Implementation Details

**Volunteer List Window**:

- A window where the user can see all volunteers in a list and filter them by their call type.

**Configuration Window**:

- A window where the user can reset and initialize the database, change the risk range, and forward the clock by a second, minute, day, month, or year.

**Volunteer Window**:

- A window that shows all volunteer details and allows the user to update or add a new volunteer.

**Call In Progress Window**:

- A read-only window that displays the values of the call in progress for a specific user.

These windows enhance the user experience by providing a clear and intuitive interface for managing volunteers and their assignments.

## 6th Stage: Post-Presentation Layer Enhancements

### Background

In this stage, we made several enhancements and improvements to the Presentation Layer (PL) and added new features to the platform.

### Implementation Details

**Volunteer Features**:

- **Access to Calls**: Volunteers can now take calls, finish them, or cancel them directly from the UI.
- **Settings and History**: Volunteers can view and modify their settings, and see their history of calls.

**Admin Features**:

- **Manage Entities**: Admins can add new calls, delete assignments, and manage every other entity in the system.

**New Features**:

- **Email Notifications**: Implemented an email sender using SMTP protocol to notify relevant volunteers when a new call is available nearby.
- **Google Maps Integration**: Added a feature to display the distance between the volunteer and the call location using Google Maps. The user can choose between air, walking, and driving distances, providing a visual perspective of the calls relative to the volunteer's location.

These enhancements improve the overall functionality and user experience of the platform, making it more versatile and user-friendly.

## Features

- **Local Database Management**: Allows the user to add new entity records to the local database stored in a simple list structure.
- **DO Entities Description**: A class implementation has been added for every entity that would be involved in the project including the Call, Assignment, and Volunteer entities.
- **Main Program for Testing Purposes**: Provides a program in which the developer can simulate the functionality of the CRUD APIs including adding, updating, removing, and retrieving entity records from the API layer.

## Getting Started

To get started with the project, clone the repository and follow the instructions in the respective stage documentation.

## Contributing

We welcome contributions from the community. Please read our [contributing guidelines](CONTRIBUTING.md) for more information on how to get involved.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
