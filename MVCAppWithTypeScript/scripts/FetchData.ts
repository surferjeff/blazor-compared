const response = await fetch("/Forecasts");
if (!response.ok) {
    throw new Error(`Response status: ${response.status}`);
}
const html = await response.text();
document.getElementById("tableHere").innerHTML = html;

export {}