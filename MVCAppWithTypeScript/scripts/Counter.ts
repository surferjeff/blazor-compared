import { gebi } from "./core.js";

let count = 0;
function onClick() {
    gebi("countSpan").innerText = String(++count);
}
gebi("countBtn").onclick = onClick;
