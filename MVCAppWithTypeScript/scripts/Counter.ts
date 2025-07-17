let count = 0;
function onClick() {
    document.getElementById("countSpan").innerText = String(count += 1);
}
document.getElementById("countBtn").onclick = onClick;
