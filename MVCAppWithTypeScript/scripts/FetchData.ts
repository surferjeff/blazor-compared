const response = await fetch("/Forecasts");
if (!response.ok) throw new Error(`Bad response: ${response.status}`);
document.getElementById("tableHere").innerHTML = await response.text();

export {}  // Declare a module so we can use top-level await above.