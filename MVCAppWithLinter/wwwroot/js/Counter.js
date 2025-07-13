let count = 0;

/**
 * Increments the value displayed in <span id="countSpan">
 */
export function onClick() {
    document.getElementById("countSpan").innerText = ++count;
}

document.getElementById("countBtn").onclick = onClick;
