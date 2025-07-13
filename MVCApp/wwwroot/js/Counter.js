let count = 0;

export function onClick() {
    document.getElementById("countSpan").innerText = ++count;
}

document.getElementById("countBtn").onclick = onClick;
