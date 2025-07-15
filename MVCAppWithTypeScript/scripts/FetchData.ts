const response = await fetch("/Forecasts");
if (!response.ok) {
    throw new Error (`Failed to fetch /Forecasts: ${response.status}, ${response.statusText}`);
}
const html = await response.text();
document.getElementById("tableHere").innerHTML = html;

export {}  // Declare a module so we can use top-level await above.