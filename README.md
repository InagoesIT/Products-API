# Products API

## Overview

This project is a simple RESTful API for managing products, implemented using ASP.NET Web APIs. The task was provided by ChatGPT, and the requirements are outlined below.

## Task Requirements

1. **CRUD Operations using Repositories:**

   - `READ` all products.
   - `CREATE` a single product.
   - `READ` a product by id.
   - `UPDATE` an existing product by id.
   - `DELETE` a product by id.

2. **Implement Endpoints:**

   - `GET /api/products`: Retrieve a list of products (using a hardcoded list for simplicity).
   - `POST /api/products`: Add a new product to the list.
   - `GET /api/products/{id}`: Retrieve a product by id.
   - `PUT /api/products/{id}`: Update a product by id.
   - `DELETE /api/products/{id}`: Delete a product by id.

3. **Data Validation:**

   - Ensure that the product model has required fields (e.g., name, price).
   - Validate input data to handle common errors gracefully.

4. **Error Handling:**

   - Handle exceptions appropriately.
   - Return meaningful error messages in the API responses.

5. **Authentication: (will be added latter)**

   - Allow only authenticated users to perform POST, PUT, and DELETE operations.
   - Use a simple username/password approach for authentication.

6. **Dependency Injection:**
   - Organize your code using dependency injection for services and controllers.

## Getting Started In progress...

Follow these steps to set up and run the project locally...

### Prerequisites In progress...

- [.NET SDK](https://dotnet.microsoft.com/download) installed.

### Installation In progress...

1. Clone the repository:
   ````bash
   git clone https://github.com/your-username/Products-API.git```
   ````

### Usage In progress...

- **To view the documentation that I wrote before coding**
  1. Open [Swagger Online Editor](https://editor.swagger.io/)
  2. Copy the swagger.yaml file contents from the main directory or from the github repository
  3. Paste the file contents into the editor
