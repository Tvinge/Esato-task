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

$value = $_POST["value"];
$method = $_POST["method"];

switch ($method) {
	case "1"://edit
		editWeather($value["ID"], $value["City"], $value["Temperature"], $value["Description"], $value["Date"], $value["IsFetched"]);
		break;
    case "2"://add
		addWeather($value["City"], $value["Temperature"], $value["Description"], $value["Date"], $value["IsFetched"]);
		break;
	case "3"://delete
		deleteWeather($value);
		break;
	default:
		echo "Invalid method";
})

function listWeather() {
    global $conn;
    $result = $conn->query("SELECT * FROM Weather ORDER BY Date DESC LIMIT 1"); // Fetch the latest entry
    $data = array();
    while ($row = $result->fetch_assoc()) {
        $row['Date'] = date("Y-m-d H:i:s", strtotime($row['Date']));
        $data[] = $row;
    }
    echo json_encode($data);
}

// Add a new entity
function addWeather($city, $temperature, $description, $date, $isFetched) {
    global $conn;
    $stmt = $conn->prepare("INSERT INTO Weather (City, Temperature, Description, Date, IsFetched) VALUES (?, ?, ?, ?, ?)");
    $stmt->bind_param("sdssi", $city, $temperature, $description, $date, $isFetched);
    $stmt->execute();
    $stmt->close();
    echo "added successfully";
}

// Edit an entity
function editWeather($id, $city, $temperature, $description, $date, $isFetched) {
    global $conn;
    $stmt = $conn->prepare("UPDATE Weather SET City=?, Temperature=?, Description=?, Date=?, IsFetched=? WHERE ID=?");
    $stmt->bind_param("sdssii", $city, $temperature, $description, $date, $isFetched, $id);
    $stmt->execute();
    $stmt->close();
    echo "updated successfully";
}

// Delete an entity
function deleteWeather($id) {
    global $conn;
    $stmt = $conn->prepare("DELETE FROM Weather WHERE ID=?");
    $stmt->bind_param("i", $id);
    $stmt->execute();
    $stmt->close();
    echo "deleted successfully";
}
?>