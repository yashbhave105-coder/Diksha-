USE disha_db;
SET NAMES utf8mb4;


-- 1. SEED CAREERS (12 IT TRACKS ONLY)
INSERT INTO careers (id, title, summary, description, roadmap, avg_salary_range, duration_to_start, cost_estimate, demand_badge, icon, is_active) VALUES
(1, 'IT Support Technician', 'Diagnose hardware/software issues and manage workplace network systems.', 'IT Support Technicians keep computers, networks, and software running smoothly for businesses. Perfect entry-level IT role requiring low initial cost and quick entry into tech.', 
'[{"step_number":1,"title":"Computer Basics & Hardware","description":"Learn Windows/Linux basics, computer assembly, and troubleshooting.","is_completed":true},{"step_number":2,"title":"Networking & OS Setup","description":"Master IP addresses, DNS, Routers, and OS installations.","is_completed":true},{"step_number":3,"title":"CompTIA A+ / Google IT Support","description":"Complete free online certificate courses via Coursera/NPTEL.","is_completed":false},{"step_number":4,"title":"Helpdesk Support Internship","description":"Apply for remote or local IT support entry-level jobs.","is_completed":false}]',
'₹18,000 - ₹28,000 / month', '3-4 Months', '₹0 - Free Certifications', 'High Hiring Demand', 'bi-tools', TRUE),

(2, 'QA & Software Tester', 'Ensure software quality by writing test cases and identifying bugs.', 'Software Testers evaluate applications to find errors before release. Requires strong analytical thinking and attention to detail without heavy coding requirements initially.',
'[{"step_number":1,"title":"Software Testing Fundamentals","description":"Understand SDLC, STLC, test plans, and bug lifecycle.","is_completed":true},{"step_number":2,"title":"Manual Testing & Jira","description":"Practice bug reporting on real open-source tools using Jira.","is_completed":true},{"step_number":3,"title":"Basic Automation & SQL","description":"Learn basic Selenium with Python/Java and database queries.","is_completed":false},{"step_number":4,"title":"QA Junior Specialist Job","description":"Build a portfolio of test cases and start applying.","is_completed":false}]',
'₹22,000 - ₹35,000 / month', '4-5 Months', '₹0 - Free Online Labs', 'High Demand', 'bi-bug', TRUE),

(3, 'Frontend Web Developer', 'Build interactive websites and user interfaces using HTML, CSS, JavaScript, and React.', 'Frontend developers craft visual web experiences. Extremely popular debt-free track with abundant remote and freelance opportunities.',
'[{"step_number":1,"title":"HTML5 & Modern CSS3","description":"Master semantic HTML, Flexbox, Grid, and responsive layout design.","is_completed":true},{"step_number":2,"title":"JavaScript & DOM Manipulation","description":"Learn ES6+, async JS, APIs, and building interactive web pages.","is_completed":true},{"step_number":3,"title":"React.js & Tailwind CSS","description":"Build modern web applications using components and state management.","is_completed":false},{"step_number":4,"title":"Portfolio Projects & Hiring","description":"Deploy 3 real-world projects on GitHub Pages and apply for jobs.","is_completed":false}]',
'₹25,000 - ₹45,000 / month', '4-6 Months', '₹0 - Free Bootcamps', 'Very High Demand', 'bi-code-slash', TRUE),

(4, 'Backend Developer (C# / Node.js)', 'Design database architecture, REST APIs, and server-side application logic.', 'Backend developers build server systems powering web apps. Focuses on logic, algorithms, databases, and security.',
'[{"step_number":1,"title":"Programming Foundations (C# / JS)","description":"Master OOP concepts, data structures, and algorithmic logic.","is_completed":true},{"step_number":2,"title":"Relational Databases & SQL","description":"Design MySQL/PostgreSQL schemas and write efficient queries.","is_completed":true},{"step_number":3,"title":"RESTful APIs & ASP.NET Core","description":"Create secure backend Web APIs with authentication.","is_completed":false},{"step_number":4,"title":"Cloud Deployment & Jobs","description":"Deploy APIs to free cloud hosting and secure junior developer roles.","is_completed":false}]',
'₹30,000 - ₹50,000 / month', '5-7 Months', '₹0 - Open Source', 'High Salary', 'bi-server', TRUE),

(5, 'Full-Stack Web Developer', 'Combine frontend visual interfaces with backend server logic and database systems.', 'Full-Stack developers handle both client and server applications. Highly valued by startups and tech firms.',
'[{"step_number":1,"title":"Frontend Mastery","description":"Build responsive interfaces with HTML, CSS, JavaScript, and Bootstrap.","is_completed":true},{"step_number":2,"title":"Backend APIs & Databases","description":"Master Node.js/C# and MySQL/MongoDB database operations.","is_completed":true},{"step_number":3,"title":"Full-Stack Integration","description":"Connect frontend components to backend endpoints securely.","is_completed":false},{"step_number":4,"title":"Startup Internship & Placement","description":"Build end-to-end applications and get hired in top tech hubs.","is_completed":false}]',
'₹32,000 - ₹55,000 / month', '6-8 Months', '₹0 - Self-Paced', 'Top Hiring Demand', 'bi-layers', TRUE),

(6, 'Data Analyst & Insights Specialist', 'Transform raw data into meaningful insights using Excel, SQL, PowerBI, and Python.', 'Data Analysts help companies make smart decisions by analyzing numbers and creating charts. Great math and logic track.',
'[{"step_number":1,"title":"Advanced Excel & Google Sheets","description":"Master lookup formulas, pivot tables, and data cleaning.","is_completed":true},{"step_number":2,"title":"SQL Data Retrieval","description":"Write complex queries, JOINs, aggregations, and subqueries.","is_completed":true},{"step_number":3,"title":"Data Visualization (PowerBI / Tableau)","description":"Build interactive dashboards for corporate stakeholders.","is_completed":false},{"step_number":4,"title":"Python for Data Analysis","description":"Learn Pandas, Matplotlib, and real dataset analysis.","is_completed":false}]',
'₹28,000 - ₹48,000 / month', '4-6 Months', '₹0 - Free Data Courses', 'In-Demand', 'bi-bar-chart-line', TRUE),

(7, 'Cybersecurity Support Analyst', 'Protect computer systems, networks, and data from cyber threats and breaches.', 'Entry-level cyber defense role focused on monitoring security alerts, vulnerability scanning, and incident response.',
'[{"step_number":1,"title":"Networking & OS Security","description":"Understand TCP/IP protocols, firewalls, Linux CLI, and permission systems.","is_completed":true},{"step_number":2,"title":"Security Fundamentals & Threats","description":"Learn malware analysis, phishing defense, and OWASP Top 10 vulnerabilities.","is_completed":true},{"step_number":3,"title":"SOC Tools & Log Analysis","description":"Use Wireshark, Splunk, and Nmap for network investigation.","is_completed":false},{"step_number":4,"title":"Security Certification & Job Entry","description":"Earn CompTIA Security+ or Google Cyber Cert for entry jobs.","is_completed":false}]',
'₹30,000 - ₹52,000 / month', '5-7 Months', '₹0 - Free Cyber Labs', 'High Security Demand', 'bi-shield-lock', TRUE),

(8, 'Cloud Support Associate', 'Manage cloud server infrastructure on AWS, Azure, or Google Cloud Platform.', 'Cloud Technicians deploy, monitor, and maintain server infrastructure in the cloud for high reliability and scalability.',
'[{"step_number":1,"title":"Linux Administration Basics","description":"Master Linux terminal commands, bash scripts, and SSH remote access.","is_completed":true},{"step_number":2,"title":"Cloud Concepts (AWS / Azure)","description":"Learn virtual machines, cloud storage buckets, and networking.","is_completed":true},{"step_number":3,"title":"AWS Cloud Practitioner / Azure Fundamentals","description":"Earn free or subsidized cloud practitioner credentials.","is_completed":false},{"step_number":4,"title":"Cloud Tech Support Role","description":"Assist enterprise clients with cloud server migration and support.","is_completed":false}]',
'₹30,000 - ₹50,000 / month', '5-6 Months', '₹0 - Free AWS Tier', 'High Salary Growth', 'bi-cloud-upload', TRUE),

(9, 'UI/UX Mobile & Web Designer', 'Design user journeys, wireframes, prototypes, and visual interfaces using Figma.', 'UI/UX designers focus on making software easy and delightful to use. Ideal for creative, visual problem solvers.',
'[{"step_number":1,"title":"UX Research & User Journeys","description":"Understand user pain points, wireframing, and usability heuristics.","is_completed":true},{"step_number":2,"title":"Figma & UI Design Systems","description":"Create high-fidelity mockups, auto-layout components, and color palettes.","is_completed":true},{"step_number":3,"title":"Interactive Prototyping","description":"Build clickable mobile and desktop app prototypes.","is_completed":false},{"step_number":4,"title":"Design Portfolio & Freelancing","description":"Showcase 3 case studies on Behance/Dribbble to land design roles.","is_completed":false}]',
'₹25,000 - ₹42,000 / month', '3-5 Months', '₹0 - Free Figma', 'Creative Tech Track', 'bi-palette', TRUE),

(10, 'Network Administrator', 'Install, configure, and maintain local area networks (LAN/WAN) and routers.', 'Network Admins keep corporate communication lines open, connecting hardware, switches, and firewalls securely.',
'[{"step_number":1,"title":"Cisco Networking Principles","description":"Learn OSI model, IP subnetting, VLANs, and Ethernet cabling.","is_completed":true},{"step_number":2,"title":"Router & Switch Configuration","description":"Configure Cisco Packet Tracer simulations and firewall rules.","is_completed":true},{"step_number":3,"title":"Network Troubleshooting","description":"Diagnose packet loss, DNS resolution failures, and VPN connections.","is_completed":false},{"step_number":4,"title":"CCNA Certification & Job Entry","description":"Earn CCNA credential and join corporate network management teams.","is_completed":false}]',
'₹24,000 - ₹40,000 / month', '4-6 Months', '₹0 - Packet Tracer Free', 'Steady Hiring', 'bi-diagram-3', TRUE),

(11, 'Mobile App Developer (Flutter/Android)', 'Create native and cross-platform mobile apps for Android devices.', 'Mobile developers write apps used on smartphones. Flutter allows building apps for both Android and iOS with one codebase.',
'[{"step_number":1,"title":"Dart / Java Fundamentals","description":"Master basic syntax, object-oriented concepts, and async tasks.","is_completed":true},{"step_number":2,"title":"Flutter UI Components","description":"Build responsive mobile layouts, lists, forms, and navigation drawers.","is_completed":true},{"step_number":3,"title":"API Integration & Local Storage","description":"Connect mobile apps to REST backend APIs and SQLite databases.","is_completed":false},{"step_number":4,"title":"Play Store Publishing & Job Search","description":"Publish a real app on Google Play Store and interview with tech firms.","is_completed":false}]',
'₹28,000 - ₹48,000 / month', '5-7 Months', '₹0 - Free Flutter Docs', 'High Mobile Growth', 'bi-phone', TRUE),

(12, 'Database Administrator (DBA)', 'Maintain database performance, backups, security permissions, and query optimization.', 'Database Administrators keep critical data safe, fast, and accessible 24/7 for organizations.',
'[{"step_number":1,"title":"Relational Database Concepts","description":"Understand ACID properties, normalization, indexing, and MySQL engine.","is_completed":true},{"step_number":2,"title":"SQL Scripts & Stored Procedures","description":"Write complex queries, triggers, views, and automated maintenance scripts.","is_completed":true},{"step_number":3,"title":"Backup, Recovery & Replication","description":"Configure automatic daily database backups and disaster recovery.","is_completed":false},{"step_number":4,"title":"Junior DBA Placement","description":"Manage database infrastructure for healthcare, fintech, or retail platforms.","is_completed":false}]',
'₹30,000 - ₹52,000 / month', '5-7 Months', '₹0 - Open Source MySQL', 'Essential Tech Role', 'bi-database', TRUE);

-- 2. SEED CAREER TAGS
INSERT INTO career_tags (career_id, tag) VALUES
(1, 'support'), (1, 'security'),
(2, 'qa'), (2, 'support'),
(3, 'frontend'), (3, 'design'),
(4, 'backend'), (4, 'cloud'),
(5, 'frontend'), (5, 'backend'),
(6, 'data'), (6, 'backend'),
(7, 'security'), (7, 'support'), (7, 'cloud'),
(8, 'cloud'), (8, 'backend'), (8, 'support'),
(9, 'design'), (9, 'frontend'),
(10, 'support'), (10, 'security'),
(11, 'frontend'), (11, 'backend'),
(12, 'data'), (12, 'backend');

-- 3. SEED FINANCIAL AID & LAPTOP SUBSIDIES
INSERT INTO financial_aid (id, title, provider, description, eligibility, amount, apply_url, applicable_states, applicable_classes, last_verified_date, is_active) VALUES
(1, 'TCS Smart Tech Opportunity Grant', 'Tata Consultancy Services Foundation', 'Provides ₹25,000 cash grant plus a free refurbished laptop to students pursuing IT diploma or certification programs.', 'Open to students with family annual income < ₹2.5 Lakhs enrolled in any IT skill track.', '₹25,000 + Free Laptop', 'https://example.org/aid/tcs-grant', '["Maharashtra","Karnataka","Delhi","Telangana","Tamil Nadu","Uttar Pradesh","West Bengal","Bihar"]', '["Class 10 Pass","Class 12 Pass","Diploma","Undergraduate"]', '2026-08-15', TRUE),
(2, 'Digital India Free Laptop Scheme', 'Ministry of Electronics & IT (MeitY)', 'Government scheme distributing free laptops with preloaded IT learning materials for underprivileged youth.', 'Students who completed 10th or 12th standard with interest in computer science / IT.', 'Free Laptop Device', 'https://example.gov.in/aid/laptop-subsidy', null, '["Class 10 Pass","Class 12 Pass","Diploma"]', '2026-09-01', TRUE),
(3, 'Google Career Certificate Scholarship', 'Google & NASSCOM Foundation', '100% tuition waiver for Google Professional Certificates in Data Analytics, IT Support, Cybersecurity, and UX Design.', 'Open to all Indian students; priority given to first-generation learners and female applicants.', '100% Tuition Waiver (₹20,000 value)', 'https://example.org/aid/google-nasscom', null, null, '2026-08-28', TRUE),
(4, 'Pradhan Mantri Gramin Digital Saksharta Abhiyan (PMGDISHA)', 'Government of India', 'Free 20-hour digital literacy and hardware setup training with certification stipend.', 'Rural household members with no prior computer exposure.', 'Free Training + ₹1,500 Stipend', 'https://pmgdisha.gov.in', null, null, '2026-07-20', TRUE),
(5, 'Infosys Springboard Tech Scholarship', 'Infosys Foundation', 'Free access to 500+ premium software development courses, cloud labs, and top-performer hardware grants.', 'Students pursuing diploma, ITI, or undergraduate studies in any Indian state.', 'Free Learning + Refurbished PC', 'https://springboard.infosys.com', null, '["Class 12 Pass","Diploma","Undergraduate"]', '2026-09-05', TRUE),
(6, 'Karnataka Free Laptop Scheme for SC/ST IT Students', 'Department of Social Welfare, Govt of Karnataka', 'State-specific free laptop distribution for eligible SC/ST students enrolling in computer training.', 'Karnataka residents enrolled in IT or polytechnic diploma programs.', 'Free High-Spec Laptop', 'https://sw.kar.nic.in/aid/laptop', '["Karnataka"]', '["Class 12 Pass","Diploma"]', '2026-08-10', TRUE),
(7, 'Maharashtra Jyotirao Phule IT Skill Grant', 'Govt of Maharashtra', 'Direct benefit transfer for IT learning software, Wi-Fi data reimbursement, and course certification fees.', 'Maharashtra resident students with family income under ₹1.8 Lakhs.', '₹12,000 Annual Allowance', 'https://mahadbt.maharashtra.gov.in', '["Maharashtra"]', '["Class 10 Pass","Class 12 Pass","Diploma"]', '2026-08-20', TRUE),
(8, 'Reliance Foundation Undergraduate Tech Scholarship', 'Reliance Foundation', 'Merit-cum-means scholarship offering up to ₹2 Lakhs over the duration of computer science / IT study.', 'Enrolled in 1st year undergraduate degree/diploma in IT, CS, or AI.', 'Up to ₹2,00,000', 'https://reliancefoundation.org/scholarship', null, '["Class 12 Pass","Undergraduate"]', '2026-08-30', TRUE),
(9, 'Wipro Equal Opportunity Tech Fellowship', 'Wipro Cares Foundation', 'Financial aid covering Internet broadband connection costs and personal computer hardware repairs.', 'Students pursuing self-paced IT roadmaps with family income < ₹2.0 Lakhs.', '₹8,000 Internet & Device Grant', 'https://wiprocares.org/tech-fellowship', null, null, '2026-09-02', TRUE),
(10, 'Delhi Ladli Digital IT Laptop Subsidy', 'Govt of NCT of Delhi', 'Financial subsidy of ₹15,000 for female students purchasing their first computer for IT learning.', 'Female residents of Delhi who passed 10th or 12th standard.', '₹15,000 Laptop Voucher', 'https://edistrict.delhigovt.nic.in', '["Delhi"]', '["Class 10 Pass","Class 12 Pass"]', '2026-08-18', TRUE);

-- 4. SEED QUIZ QUESTIONS & OPTIONS
INSERT INTO quiz_questions (id, question_text, order_index, is_active) VALUES
(1, 'What kind of daily tech activities sound most exciting to you?', 1, TRUE),
(2, 'How do you prefer to solve problems when working on a project?', 2, TRUE),
(3, 'What is your comfort level with computer hardware and physical devices?', 3, TRUE),
(4, 'If you were building a mobile app or website, which part would you focus on first?', 4, TRUE),
(5, 'How do you feel about working with numbers, spreadsheets, and data patterns?', 5, TRUE),
(6, 'Which environment sounds like the best workplace for you?', 6, TRUE),
(7, 'What type of non-technical hobby or activity do you enjoy most?', 7, TRUE),
(8, 'How do you prefer to learn new technical skills?', 8, TRUE),
(9, 'How do you react when a software application crashes or shows an error message?', 9, TRUE),
(10, 'Which tech goal matters most to you right now?', 10, TRUE),
(11, 'How interested are you in cybersecurity and defending computer networks from hackers?', 11, TRUE),
(12, 'How do you feel about building cloud servers and managing online infrastructure?', 12, TRUE);

INSERT INTO quiz_options (question_id, option_text, tag_weights) VALUES
-- Q1
(1, 'Designing beautiful visual web screens and layouts that users see', '{"frontend":3, "design":3}'),
(1, 'Writing core server logic, database rules, and secret algorithms', '{"backend":3, "data":2}'),
(1, 'Analyzing charts, statistics, and business numbers to find hidden facts', '{"data":3, "backend":1}'),
(1, 'Fixing broken computer hardware, internet connections, and printer setups', '{"support":3, "security":1}'),

-- Q2
(2, 'I like drawing quick visual sketches and testing user experience designs', '{"design":3, "frontend":2}'),
(2, 'I systematically break down complex logical puzzles step-by-step', '{"backend":3, "data":2}'),
(2, 'I inspect details carefully to spot errors, bugs, or missing steps', '{"qa":3, "security":2}'),
(2, 'I like talking directly to people to help fix their tech confusion', '{"support":3, "qa":1}'),

-- Q3
(3, 'I love opening computer cases, connecting wires, and setting up routers', '{"support":3, "cloud":2}'),
(3, 'I prefer working inside software applications rather than physical hardware', '{"frontend":2, "backend":2, "data":2}'),
(3, 'I like setting up remote server systems and internet cloud network settings', '{"cloud":3, "security":2}'),
(3, 'I haven''t handled physical hardware much, but I am excited to learn software', '{"frontend":2, "qa":2}'),

-- Q4
(4, 'The buttons, colors, animations, and how easy it is for users to tap', '{"frontend":3, "design":3}'),
(4, 'The fast login system, secure password database, and payment processing', '{"backend":3, "security":2}'),
(4, 'Testing every single feature to ensure nothing breaks under heavy usage', '{"qa":3, "support":1}'),
(4, 'Storing user information safely and running performance analytics', '{"data":3, "cloud":2}'),

-- Q5
(5, 'I enjoy calculating statistics, finding trends, and displaying visual graphs', '{"data":3}'),
(5, 'I prefer creative visual design over mathematical numbers', '{"design":3, "frontend":2}'),
(5, 'I like logic and equations if they help me code server functionality', '{"backend":3, "data":1}'),
(5, 'I prefer practical hands-on troubleshooting over heavy math calculations', '{"support":3, "qa":2}'),

-- Q6
(6, 'Fast-paced startup creating fresh website user interfaces', '{"frontend":3, "design":2}'),
(6, 'Corporate IT department managing enterprise databases and APIs', '{"backend":3, "data":2}'),
(6, 'Cybersecurity operations center monitoring cyber threat attacks', '{"security":3, "cloud":2}'),
(6, 'Helpdesk support desk assisting non-technical staff with computers', '{"support":3, "qa":1}'),

-- Q7
(7, 'Drawing, photography, UI design, or craft work', '{"design":3, "frontend":2}'),
(7, 'Solving Sudoku, chess, or complex logic puzzles', '{"backend":3, "data":2}'),
(7, 'Reading mystery detective stories and hunting for hidden clues', '{"security":3, "qa":3}'),
(7, 'Helping friends fix gadget settings or gaming console connections', '{"support":3, "cloud":1}'),

-- Q8
(8, 'Building visual websites step-by-step following video tutorials', '{"frontend":3, "design":1}'),
(8, 'Experimenting with code, database queries, and script commands', '{"backend":3, "data":2}'),
(8, 'Testing interactive online labs, terminal commands, and network tools', '{"cloud":3, "security":3}'),
(8, 'Following structured checklist manuals and step-by-step guides', '{"qa":3, "support":2}'),

-- Q9
(9, 'I get curious and inspect the exact error line to find the bug', '{"backend":3, "qa":3}'),
(9, 'I search online forums to find a solution and help user resolve it', '{"support":3, "qa":2}'),
(9, 'I check if the issue is caused by server overload or network security', '{"cloud":3, "security":3}'),
(9, 'I redesign the user interface so the user never gets confused by errors', '{"design":3, "frontend":2}'),

-- Q10
(10, 'Get a high-paying job as a Web / Software Developer', '{"frontend":3, "backend":3}'),
(10, 'Start working quickly in an IT Support or Quality Assurance role', '{"support":3, "qa":3}'),
(10, 'Become a Data Analyst or Database Administrator', '{"data":3, "backend":2}'),
(10, 'Specialize in Cybersecurity or Cloud Computing infrastructure', '{"security":3, "cloud":3}'),

-- Q11
(11, 'Very interested! I want to protect systems and stop hackers', '{"security":3, "cloud":2}'),
(11, 'Somewhat interested in security basics, but prefer web development', '{"frontend":2, "backend":2}'),
(11, 'I prefer handling database analytics and business reports', '{"data":3}'),
(11, 'I am mostly interested in computer hardware and user tech support', '{"support":3}'),

-- Q12
(12, 'Excited to host websites on Amazon AWS or Microsoft Azure cloud', '{"cloud":3, "backend":2}'),
(12, 'I want to focus on frontend web pages and mobile app screens', '{"frontend":3, "design":2}'),
(12, 'I prefer writing automated test scripts and finding app bugs', '{"qa":3}'),
(12, 'I want to assist everyday computer users with technical issues', '{"support":3}');
