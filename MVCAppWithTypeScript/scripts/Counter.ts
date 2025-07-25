let count = 0;
document.getElementById("countBtn").onclick =
    () => document.getElementById("countSpan").innerText = String(++count);
