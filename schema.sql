CREATE DATABASE IF NOT EXISTS disha_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE disha_db;

DROP TABLE IF EXISTS checklist_items;
DROP TABLE IF EXISTS mentor_chat_messages;
DROP TABLE IF EXISTS bookmarks;
DROP TABLE IF EXISTS financial_aid;
DROP TABLE IF EXISTS quiz_responses;
DROP TABLE IF EXISTS quiz_options;
DROP TABLE IF EXISTS quiz_questions;
DROP TABLE IF EXISTS career_tags;
DROP TABLE IF EXISTS careers;
DROP TABLE IF EXISTS users;

CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(150),
    role ENUM('student','admin') NOT NULL DEFAULT 'student',
    education_level VARCHAR(50),
    prior_computer_exposure ENUM('none','some','a_lot') DEFAULT 'none',
    state VARCHAR(100),
    language_pref VARCHAR(10) DEFAULT 'en',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE careers (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(150) NOT NULL,
    summary VARCHAR(300) NOT NULL,
    description TEXT NOT NULL,
    roadmap JSON NOT NULL,
    avg_salary_range VARCHAR(100),
    duration_to_start VARCHAR(100),
    cost_estimate VARCHAR(100),
    demand_badge VARCHAR(50),
    icon VARCHAR(50),
    is_active BOOLEAN DEFAULT TRUE
);

CREATE TABLE career_tags (
    id INT AUTO_INCREMENT PRIMARY KEY,
    career_id INT NOT NULL,
    tag VARCHAR(30) NOT NULL,
    FOREIGN KEY (career_id) REFERENCES careers(id) ON DELETE CASCADE
);

CREATE TABLE quiz_questions (
    id INT AUTO_INCREMENT PRIMARY KEY,
    question_text VARCHAR(300) NOT NULL,
    order_index INT NOT NULL,
    is_active BOOLEAN DEFAULT TRUE
);

CREATE TABLE quiz_options (
    id INT AUTO_INCREMENT PRIMARY KEY,
    question_id INT NOT NULL,
    option_text VARCHAR(200) NOT NULL,
    tag_weights JSON NOT NULL,
    FOREIGN KEY (question_id) REFERENCES quiz_questions(id) ON DELETE CASCADE
);

CREATE TABLE quiz_responses (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    question_id INT NOT NULL,
    option_id INT NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (question_id) REFERENCES quiz_questions(id),
    FOREIGN KEY (option_id) REFERENCES quiz_options(id)
);

CREATE TABLE financial_aid (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    provider VARCHAR(150) NOT NULL,
    description TEXT NOT NULL,
    eligibility TEXT NOT NULL,
    amount VARCHAR(100),
    apply_url VARCHAR(300),
    applicable_states JSON,
    applicable_classes JSON,
    last_verified_date DATE,
    is_active BOOLEAN DEFAULT TRUE
);

CREATE TABLE bookmarks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    item_type ENUM('career','aid') NOT NULL,
    item_id INT NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uq_bookmark (user_id, item_type, item_id),
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE mentor_chat_messages (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    sender ENUM('user','ai') NOT NULL,
    message TEXT NOT NULL,
    provider_used VARCHAR(20),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE checklist_items (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    item_text VARCHAR(200) NOT NULL,
    is_done BOOLEAN DEFAULT FALSE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);
