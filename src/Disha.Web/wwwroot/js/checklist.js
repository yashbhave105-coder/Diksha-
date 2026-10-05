/* DISHA ACTION CHECKLIST SCRIPT */
document.addEventListener('DOMContentLoaded', function () {
    const checkboxes = document.querySelectorAll('.checklist-checkbox');
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');

    checkboxes.forEach(chk => {
        chk.addEventListener('change', async function () {
            const itemId = this.dataset.id;
            const parentItem = this.closest('.checklist-item');

            if (this.checked) {
                parentItem.classList.add('done');
            } else {
                parentItem.classList.remove('done');
            }

            try {
                await fetch('/Checklist/ToggleItem', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': tokenInput ? tokenInput.value : ''
                    },
                    body: JSON.stringify({ id: parseInt(itemId) })
                });
            } catch (e) {
                console.error("Checklist update failed", e);
            }
        });
    });
});
