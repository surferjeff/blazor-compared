let count = 0;
function onClick() {
    document.getElementById("countSpan").innerText = String(++count);
}
document.getElementById("countBtn").onclick = onClick;
