import { fetchOrThrow, gebi } from "./core.js";

const response = await fetchOrThrow("/Forecasts");
const html = await response.text();
gebi("tableHere").innerHTML = html;

export {}  // Declare a module so we can use top-level await above.