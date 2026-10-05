/* DISHA BOOKMARK TOGGLE SCRIPT */
document.addEventListener('DOMContentLoaded', function () {
    const bookmarkBtns = document.querySelectorAll('.btn-bookmark-toggle');
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');

    bookmarkBtns.forEach(btn => {
        btn.addEventListener('click', async function (e) {
            e.preventDefault();
            const itemType = this.dataset.type;
            const itemId = this.dataset.id;
            const icon = this.querySelector('i');

            try {
                const res = await fetch('/Bookmark/Toggle', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': tokenInput ? tokenInput.value : ''
                    },
                    body: JSON.stringify({ itemType: itemType, itemId: parseInt(itemId) })
                });

                if (res.ok) {
                    const data = await res.json();
                    if (data.success) {
                        if (data.isBookmarked) {
                            this.classList.remove('btn-outline-secondary');
                            this.classList.add('btn-warning');
                            if (icon) icon.className = 'bi bi-bookmark-fill me-1';
                        } else {
                            this.classList.remove('btn-warning');
                            this.classList.add('btn-outline-secondary');
                            if (icon) icon.className = 'bi bi-bookmark me-1';
                        }

                        // Update dashboard bookmark count if element exists
                        const countSpan = document.getElementById('savedResourcesCountSpan');
                        if (countSpan && data.totalCount !== undefined) {
                            countSpan.innerText = `${data.totalCount} Saved`;
                        }
                    }
                }
            } catch (err) {
                console.error("Bookmark toggle failed", err);
            }
        });
    });
});
