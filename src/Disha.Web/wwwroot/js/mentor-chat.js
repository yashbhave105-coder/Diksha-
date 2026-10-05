/* DISHA AI MENTOR CHAT SCRIPT */
document.addEventListener('DOMContentLoaded', function () {
    const chatForm = document.getElementById('mentorChatForm');
    const chatInput = document.getElementById('mentorMessageInput');
    const chatBox = document.getElementById('mentorChatBox');
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');

    if (chatForm && chatInput && chatBox) {
        chatForm.addEventListener('submit', async function (e) {
            e.preventDefault();

            const message = chatInput.value.trim();
            if (!message) return;

            // 1. Append user message to chat box immediately
            appendMessage('user', message, null);
            chatInput.value = '';

            // 2. Append typing indicator
            const typingIndicator = appendTypingIndicator();
            scrollToBottom();

            try {
                const response = await fetch('/Mentor/SendMessage', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': tokenInput ? tokenInput.value : ''
                    },
                    body: JSON.stringify({ message: message })
                });

                if (typingIndicator) typingIndicator.remove();

                if (response.ok) {
                    const data = await response.json();
                    if (data.success) {
                        appendMessage('ai', data.reply, data.provider);
                    } else {
                        appendMessage('ai', 'Sorry, I could not process your request. Please try again.', 'fallback');
                    }
                } else {
                    appendMessage('ai', 'Connection error. Please try again in a moment.', 'fallback');
                }
            } catch (err) {
                if (typingIndicator) typingIndicator.remove();
                appendMessage('ai', 'An unexpected error occurred. Please check your internet connection.', 'fallback');
            }

            scrollToBottom();
        });
    }

    // Voice Input & Audio Synthesis
    const micBtn = document.getElementById('mentorMicBtn');
    const langSelect = document.getElementById('mentorSpeechLang');
    const autoReadAloudToggle = document.getElementById('autoReadAloudToggle');

    if (micBtn && ('webkitSpeechRecognition' in window || 'SpeechRecognition' in window)) {
        const SpeechRec = window.SpeechRecognition || window.webkitSpeechRecognition;
        const voiceRec = new SpeechRec();
        voiceRec.continuous = false;
        voiceRec.interimResults = false;

        voiceRec.onstart = function () {
            micBtn.className = 'btn btn-danger px-3 py-3 rounded-3 text-white';
            micBtn.innerHTML = '<i class="bi bi-record-circle fs-5"></i>';
        };

        voiceRec.onresult = function (event) {
            const transcript = event.results[0][0].transcript;
            if (chatInput) {
                chatInput.value = chatInput.value ? chatInput.value + ' ' + transcript : transcript;
            }
        };

        voiceRec.onend = function () {
            micBtn.className = 'btn btn-outline-danger px-3 py-3 rounded-3';
            micBtn.innerHTML = '<i class="bi bi-mic-fill fs-5"></i>';
        };

        micBtn.addEventListener('click', function () {
            voiceRec.lang = langSelect ? langSelect.value : 'en-IN';
            voiceRec.start();
        });
    }

    function speakText(text) {
        if ('speechSynthesis' in window && autoReadAloudToggle && autoReadAloudToggle.checked) {
            window.speechSynthesis.cancel();
            const utterance = new SpeechSynthesisUtterance(text);
            utterance.lang = langSelect ? langSelect.value : 'en-IN';
            window.speechSynthesis.speak(utterance);
        }
    }

    function appendMessage(sender, text, provider) {
        const msgDiv = document.createElement('div');
        msgDiv.className = `chat-msg ${sender}`;

        const textPara = document.createElement('div');
        textPara.innerText = text;
        msgDiv.appendChild(textPara);

        if (sender === 'ai') {
            const badge = document.createElement('span');
            badge.className = 'provider-badge';
            badge.innerText = 'AI MENTOR';
            msgDiv.appendChild(badge);

            // Audio speak button per message
            const speakBtn = document.createElement('button');
            speakBtn.className = 'btn btn-sm btn-link text-decoration-none text-muted p-0 ms-2';
            speakBtn.innerHTML = '<i class="bi bi-volume-up"></i> Listen';
            speakBtn.onclick = () => {
                if ('speechSynthesis' in window) {
                    window.speechSynthesis.cancel();
                    const u = new SpeechSynthesisUtterance(text);
                    u.lang = langSelect ? langSelect.value : 'en-IN';
                    window.speechSynthesis.speak(u);
                }
            };
            msgDiv.appendChild(speakBtn);

            speakText(text);
        }

        chatBox.appendChild(msgDiv);
        scrollToBottom();
    }

    function appendTypingIndicator() {
        const typingDiv = document.createElement('div');
        typingDiv.className = 'chat-msg ai typing-indicator';
        typingDiv.innerHTML = '<em>AI Mentor is thinking...</em>';
        chatBox.appendChild(typingDiv);
        return typingDiv;
    }

    function scrollToBottom() {
        if (chatBox) {
            chatBox.scrollTop = chatBox.scrollHeight;
        }
    }
});
