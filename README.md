# Volunteer Management Platform - Mini Project in Windows Systems

## Overview

Welcome to the Volunteer Management Platform! This project is designed to help organizations manage volunteers and the calls they can assign to themselves. The platform provides a call recommendation feature that updates based on the user's geographical location and allows setting a maximum distance for call assignments.

The project is built using a multi-tier architecture, with each week representing a stage in the development process. Each stage is tagged with a release for easy reference.

### Stages

- [0th Stage: Initialization](#0th-stage-initialization)
- [1st Stage: Data Access Layer](#1st-stage-data-access-layer)

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

## Features

- **Local Database Management**: Allows the user to add new entity records to the local database stored in a simple list structure.
- **DO Entities Description**: A class implementation has been added for every entity that would be involved in the project including the Call, Assignment, and Volunteer entities.
- **Main Program for Testing Purposes**: Provides a program in which the developer can simulate the functionality of the CRUD APIs including adding, updating removing the retrieving entity records from the API layer.

## Getting Started

To get started with the project, clone the repository and follow the instructions in the respective stage documentation.

## Contributing

We welcome contributions from the community. Please read our [contributing guidelines](CONTRIBUTING.md) for more information on how to get involved.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

Thank you for your interest in the Volunteer Management Platform! We hope this project helps you effectively manage your volunteers and their assignments.
