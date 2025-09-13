# Minimum Spanning Tree Using Kruskal's Algorithm

## 📋 Project Description

This project is an ASP.NET Core web application that implements Kruskal’s algorithm to compute the Minimum Spanning Tree (MST). Users can input their graphs, calculate the MST, view the results visually, and save them for future reference.

## ✨ Key Features

- **Kruskal’s Algorithm**: Full implementation of Kruskal’s algorithm for MST calculation
    
- **Authentication System**: User registration and login
    
- **Graph Management**: Save, view, and delete user graphs
    
- **JSON Input**: Support for graph input in JSON format
    
- **Visual Display**: Graph and MST visualization
    
- **Download Results**: Ability to download graphs and MST as JSON files
    
- **History**: Maintain user graph history
    

## 🛠️ Technologies Used

- **ASP.NET Core 8.0**: Main web framework
    
- **Entity Framework Core**: ORM for database operations
    
- **SQL Server**: Primary database
    
- **ASP.NET Core Identity**: Authentication system
    
- **Bootstrap**: CSS framework for UI
    
- **jQuery**: JavaScript library
    

## 📦 Project Structure

```
MinimumSpanningTreeWithKruskal/
├── Controllers/          # MVC Controllers  
├── Data/                 # Database-related classes  
├── Interfaces/           # Service interfaces  
├── Models/               # Data models  
├── Repositories/         # Data access layer  
├── Services/             # Business logic services  
├── ViewModels/           # View models  
├── Views/                # Razor views  
└── wwwroot/              # Static files  
```

## 🚀 How to Run

### Prerequisites

- .NET 8.0 SDK
    
- SQL Server (or SQL Server Express)
    
- Visual Studio 2022 or VS Code
    

### Installation & Running

1. **Clone the project**
    
    ```bash
    git clone https://github.com/yourusername/MinimumSpanningTreeWithKruskal.git
    cd MinimumSpanningTreeWithKruskal
    ```
    
2. **Restore packages**
    
    ```bash
    dotnet restore
    ```
    
3. **Configure the database**
    
    - Set your connection string in `appsettings.json`
        
    - Run migrations:
        
    
    ```bash
    dotnet ef database update
    ```
    
    - Alternatively, you can create the database using the SQL script in `SQLScript/file.sql` by executing it in SQL Server.
        
4. **Run the project**
    
    ```bash
    dotnet run
    ```
    
5. **Access the app**
    
    - Open your browser and go to `https://localhost:5001`
        

## 📊 How to Use

### 1. Register & Login

- Register a new account
    
- Login with your credentials
    

### 2. Create a Graph

- Go to the main page
    
- Input your graph in JSON format
    

### Graph JSON Format Example:

```json
{
  "graphName": "Sample Graph",
  "nodes": [
    {"id": 1, "label": "A"},
    {"id": 2, "label": "B"},
    {"id": 3, "label": "C"}
  ],
  "edges": [
    {"source": "A", "target": "B", "weight": 5},
    {"source": "B", "target": "C", "weight": 3},
    {"source": "A", "target": "C", "weight": 7}
  ]
}
```

### 3. Calculate MST

- After saving, click the "Calculate MST" button
    
- Kruskal’s algorithm will run and calculate the MST
    

### 4. View Results

- Results are displayed visually
    
- Compare the original graph with the MST
    

### 5. Download Results

- Download the graph and MST as JSON files
    

## 🔧 About Kruskal’s Algorithm

Kruskal’s algorithm is a popular MST algorithm:

1. **Sort edges** by ascending weight
    
2. **Select edges** starting from the smallest, avoiding cycles
    
3. **Cycle detection** uses a Disjoint Set data structure
    
4. **Repeat** until (n-1) edges are selected (n = number of nodes)
    

## 📁 Important Files

- `Services/KruskalMSTAlgorithm.cs`: Implementation of Kruskal’s algorithm
    
- `Services/DisjointSet.cs`: Data structure for cycle detection
    
- `Controllers/GraphController.cs`: Main controller for graph management
    
- `Models/Graph.cs`: Graph model
    
- `Models/Edge.cs`: Edge model
    

## 🤝 Contributing

1. Fork the repo
    
2. Create a new branch (`git checkout -b feature/amazing-feature`)
    
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
    
4. Push to your branch (`git push origin feature/amazing-feature`)
    
5. Open a Pull Request
    

## 📝 License

This project is licensed under the MIT License. See the `LICENSE` file for details.

## 📞 Contact

For questions or suggestions:

- Email: [pooria.azad53@gmail.com](mailto:pooria.azad53@gmail.com)
    
- GitHub Issues: [Create Issue](https://github.com/pouria-azad/MinimumSpanningTreeWithKruskal/issues)
    

## 🙏 Acknowledgments

Thanks to everyone who contributed to this project.

---

**Note:** This project is designed for educational and research purposes.

---
