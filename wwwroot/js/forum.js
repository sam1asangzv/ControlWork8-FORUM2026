(() => {
  const section = document.querySelector('#reply-section');
  const form = document.querySelector('#reply-form');
  if (!section) return;

  async function loadPage(topicId, page, scroll = false) {
    const response = await fetch(`/Replies/Page?topicId=${topicId}&page=${page}`, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
    if (!response.ok) throw new Error('Не удалось загрузить ответы');
    const heading = section.querySelector('h2');
    section.innerHTML = '';
    section.append(heading);
    section.insertAdjacentHTML('beforeend', await response.text());
    if (scroll) section.querySelector('.reply:last-of-type')?.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  section.addEventListener('click', async event => {
    const voteButton = event.target.closest('[data-vote]');
    if (voteButton && !voteButton.disabled) {
      const reply = voteButton.closest('.reply');
      const token = form?.querySelector('[name="__RequestVerificationToken"]')?.value;
      const body = new URLSearchParams({ replyId: reply.dataset.replyId, value: voteButton.dataset.vote, __RequestVerificationToken: token });
      const response = await fetch('/Replies/Vote', { method: 'POST', body, headers: { 'X-Requested-With': 'XMLHttpRequest' } });
      if (response.ok) {
        const data = await response.json();
        reply.querySelector('[data-vote="1"] span').textContent = data.likes;
        reply.querySelector('[data-vote="-1"] span').textContent = data.dislikes;
      }
      return;
    }
    const link = event.target.closest('.reply-pages a');
    if (!link) return;
    event.preventDefault();
    const nav = link.closest('.reply-pages');
    try { await loadPage(nav.dataset.topic, link.dataset.page, true); history.replaceState(null, '', link.href); }
    catch (error) { alert(error.message); }
  });

  form?.addEventListener('submit', async event => {
    event.preventDefault();
    const status = form.querySelector('.form-status');
    const button = form.querySelector('button');
    button.disabled = true; status.textContent = 'Отправка...';
    try {
      const response = await fetch('/Replies/Send', { method: 'POST', body: new FormData(form), headers: { 'X-Requested-With': 'XMLHttpRequest' } });
      const data = await response.json();
      if (!response.ok) throw new Error(data.message || 'Не удалось отправить ответ');
      const topicId = form.elements.topicId.value;
      form.elements.content.value = '';
      await loadPage(topicId, data.page, true);
      status.textContent = 'Ответ опубликован';
    } catch (error) { status.textContent = error.message; }
    finally { button.disabled = false; }
  });
})();
