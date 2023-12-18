# Products API

## Overview
This project is a simple RESTful API for managing products, implemented using ASP.NET Web APIs. The task was provided by ChatGPT, and the requirements are outlined below.

## Task Requirements
1. **Implement Endpoints:**
    - `GET /api/products`: Retrieve a list of products (using a hardcoded list for simplicity).
    - `POST /api/products`: Add a new product to the list.

2. **Data Validation:**
    - Ensure that the product model has required fields (e.g., name, price).
    - Validate input data to handle common errors gracefully.

3. **CRUD Operations:**
    - `GET` a single product by ID.
    - `PUT` to update an existing product.
    - `DELETE` to remove a product.

4. **Error Handling:**
    - Handle exceptions appropriately.
    - Return meaningful error messages in the API responses.

5. **Authentication:**
    - Allow only authenticated users to perform POST, PUT, and DELETE operations.
    - Use a simple username/password approach for authentication.

6. **Dependency Injection:**
    - Organize your code using dependency injection for services and controllers.

## Getting Started
Follow these steps to set up and run the project locally... TBA

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) installed. TBA

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/Products-API.git```