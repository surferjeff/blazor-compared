/**
 * @typedef {object} ListNode
 * @property {0 | 1} val - The numerical value stored in the node.
 * @property {ListNode?} next - A reference to the next node in the list, or undefined if this is the last node.
 */

/**
 * Converts digits from the linked list into a number.
 * @param {ListNode} head - The first node in the list.
 * @returns {number} - The decimal value.
 */
function getDecimalValue(head) {
    let n = 0;
    while (head) {
        n = (n << 1) | head.val;
        head = head.next;
        head.q = 10;
    }
    return n;
};

/**
 * Demonstrates type-safety enforced by JSDoc.
 */
export function test() {
    /** type: { ListNode } */
    let list = { val: 1, next: { val: 0, next: { val: 'a' } } };
    console.log(getDecimalValue(list));
}