// KursAL — Site JavaScript

// User dropdown toggle
function toggleUserMenu() {
    const dd = document.getElementById('userDropdown');
    if (dd) dd.classList.toggle('open');
}

// Close dropdown on outside click
document.addEventListener('click', (e) => {
    if (!e.target.closest('.user-menu')) {
        document.getElementById('userDropdown')?.classList.remove('open');
    }
});

// Mobile nav toggle
function toggleNav() {
    const links = document.querySelector('.nav-links');
    if (links) links.style.display = links.style.display === 'flex' ? 'none' : 'flex';
}

// Auto-dismiss toasts
document.addEventListener('DOMContentLoaded', () => {
    const toasts = document.querySelectorAll('.toast-success, .toast-error');
    toasts.forEach(toast => {
        setTimeout(() => {
            toast.style.opacity = '0';
            toast.style.transition = 'opacity 0.4s';
            setTimeout(() => toast.remove(), 400);
        }, 4000);
    });

    // Role option click
    document.querySelectorAll('.role-option').forEach(opt => {
        opt.addEventListener('click', () => {
            document.querySelectorAll('.role-option').forEach(o => o.classList.remove('role-active'));
            opt.classList.add('role-active');
            const radio = opt.querySelector('input[type="radio"]');
            if (radio) radio.checked = true;
        });
    });

    // Filter radio labels
    document.querySelectorAll('.filter-radio input').forEach(radio => {
        radio.addEventListener('change', () => {
            document.querySelectorAll('.filter-radio').forEach(l => l.classList.remove('active'));
            radio.closest('.filter-radio')?.classList.add('active');
        });
    });

    // Star hover effect on rating
    const stars = document.querySelectorAll('.star-pick');
    stars.forEach((star, idx) => {
        star.addEventListener('mouseenter', () => {
            stars.forEach((s, i) => {
                s.style.color = i <= idx ? 'var(--accent-3)' : 'var(--border-light)';
            });
        });
        star.addEventListener('mouseleave', () => {
            const picked = parseInt(document.getElementById('starsInput')?.value || '0');
            stars.forEach((s, i) => {
                s.style.color = '';
                s.classList.toggle('picked', i < picked);
            });
        });
    });
});

// Global pickStar function for inline use
function pickStar(val) {
    const input = document.getElementById('starsInput');
    if (input) input.value = val;
    document.querySelectorAll('.star-pick').forEach((s, i) => {
        s.classList.toggle('picked', i < val);
    });
}

// Toggle password visibility
function togglePass() {
    const input = document.getElementById('passInput');
    const icon = document.getElementById('eyeIcon');
    if (!input) return;
    input.type = input.type === 'password' ? 'text' : 'password';
    if (icon) icon.className = input.type === 'password' ? 'fa fa-eye' : 'fa fa-eye-slash';
}
