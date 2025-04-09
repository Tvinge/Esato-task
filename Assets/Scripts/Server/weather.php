<?php
error_reporting(E_ALL);
ini_set('display_errors', 1);

$servername = "localhost";
$username = "root";
$password = "root";
$dbname = "weatherdb";
$apiKey = "bb426a56953a694b517bf41b50f2bd7b";
$city = "London";

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

class Weather {
    public $ID;
    public $City;
    public $Temperature;
    public $Description;
    public $Date;
    public $dt;
    public $IsFetched;
}

// fetch weather data from OpenWeather API
function fetchWeatherData($city, $apiKey) {
    $url = "https://api.openweathermap.org/data/2.5/weather?q={$city}&appid={$apiKey}&units=metric";
    $response = file_get_contents($url);
    if ($response === FALSE) {
        die("Error fetching weather data.");
    }
    return json_decode($response, true);
}

// Function to insert weather data into the database
function insertWeatherData($city, $temperature, $description, $date, $isFetched) {
    global $conn;
    $stmt = $conn->prepare("INSERT INTO Weather (City, Temperature, Description, Date, IsFetched) VALUES (?, ?, ?, ?, ?)");
    $stmt->bind_param("sdssi", $city, $temperature, $description, $date, $isFetched);
    $stmt->execute();
    $stmt->close();
}

// Fetch weather data
$weatherData = fetchWeatherData($city, $apiKey);

if ($weatherData && $weatherData['cod'] == 200) {
    $city = $weatherData['name'];
    $temperature = $weatherData['main']['temp'];
    $description = $weatherData['weather'][0]['description'];
    $date = date("Y-m-d H:i:s");
    $dt = $weatherData['dt'];
    $isFetched = true;

    // Insert weather data into the database
    insertWeatherData($city, $temperature, $description, $date, $isFetched);

    //echo "Weather data for {$city} has been inserted into the database.";
        // Return the JSON response
    header('Content-Type: application/json');
    echo json_encode($weatherData);
} 
else {
    echo "Failed to fetch weather data.";
}


$conn->close();
?>