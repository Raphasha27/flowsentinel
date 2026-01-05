document.addEventListener('DOMContentLoaded', () => {
    // Initialize Lucide Icons
    lucide.createIcons();

    // Tab Switching Logic
    const navLinks = document.querySelectorAll('.nav-links li');
    const tabContents = document.querySelectorAll('.tab-content');
    const pageTitle = document.getElementById('page-title');
    const pageSubtitle = document.getElementById('page-subtitle');

    const tabMetadata = {
        overview: { title: 'Command Center', subtitle: 'Real-time traffic governance and policy enforcement.' },
        policies: { title: 'Traffic Policies', subtitle: 'Define and manage granular rate limits and security rules.' },
        analytics: { title: 'Fleet Analytics', subtitle: 'Deep dive into traffic patterns and usage metrics.' },
        abuse: { title: 'Threat Intelligence', subtitle: 'Automated abuse detection and anomaly scoring.' },
        settings: { title: 'System Settings', subtitle: 'Configure core sentinel clusters and global defaults.' }
    };

    navLinks.forEach(link => {
        link.addEventListener('click', () => {
            const tabId = link.getAttribute('data-tab');

            // Update Sidebar
            navLinks.forEach(l => l.classList.remove('active'));
            link.classList.add('active');

            // Update Content
            tabContents.forEach(content => {
                content.classList.remove('active');
                if (content.id === tabId) {
                    content.classList.add('active');
                }
            });

            // Update Header
            if (tabMetadata[tabId]) {
                pageTitle.textContent = tabMetadata[tabId].title;
                pageSubtitle.textContent = tabMetadata[tabId].subtitle;
            }
        });
    });

    // Simulate Real-time Stats Updates
    const totalReqEl = document.getElementById('total-req');
    const blockedReqEl = document.getElementById('blocked-req');
    const avgLatencyEl = document.getElementById('avg-latency');

    let totalRequests = 1245600;
    let totalBlocked = 4230;

    setInterval(() => {
        // Random incremental traffic
        const newReq = Math.floor(Math.random() * 5);
        const newBlocked = Math.random() > 0.9 ? 1 : 0;

        totalRequests += newReq;
        totalBlocked += newBlocked;

        totalReqEl.textContent = (totalRequests / 1000000).toFixed(2) + 'M';
        blockedReqEl.textContent = (totalBlocked / 1000).toFixed(1) + 'k';

        // Slightly fluctuate latency
        const latency = 20 + Math.floor(Math.random() * 8);
        avgLatencyEl.textContent = latency + 'ms';
        
        // Randomize chart bars slightly to look "alive"
        const bars = document.querySelectorAll('.bar');
        bars.forEach(bar => {
            if (Math.random() > 0.7) {
                const currentHeight = parseInt(bar.style.height);
                const delta = (Math.random() * 10) - 5;
                const newHeight = Math.min(Math.max(currentHeight + delta, 20), 95);
                bar.style.height = newHeight + '%';
            }
        });
    }, 2000);

    // Add some "Entrance" logs to the console for senior engineering feel
    console.log('%c🚦 FlowSentinel Dashboard Initialized', 'color: #0076ff; font-size: 20px; font-weight: bold;');
    console.log('%cConnected to Control Plane: %clocalhost:5000', 'color: #a8a8a8;', 'color: #0076ff;');
    console.log('%cCluster: %cFS-EPU-WEST-01', 'color: #a8a8a8;', 'color: #8a3ffc;');
});
