-- Seed data: master/lookup + demo users
-- Dijalankan setelah 01-schema.sql (docker-entrypoint-initdb.d urut alfabet).

-- pgcrypto: hashing bcrypt via crypt()/gen_salt('bf'), kompatibel dengan BCrypt.Net di app.
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- 1. Roles (is_system=true utk role yang dilindungi dari hapus)
INSERT INTO roles (name, is_system) VALUES
('admin',      true),
('manager',    false),
('employee',   true),
('supervisor', false),
('reviewer',   false);

-- 2. User statuses
INSERT INTO user_statuses (name) VALUES
('active'),
('inactive');

-- 3. Condition statuses
INSERT INTO condition_statuses (name) VALUES
('active'),
('inactive');

-- 4. Demo users (password: demo1234)
INSERT INTO users (nip, fullname, email, password, status_id, condition_status_id, phone) VALUES
('NIP1001', 'admin',    'admin@company.com',    crypt('demo1234', gen_salt('bf')), 1, 1, '+6281234567890'),
('NIP1002', 'employee', 'employee@company.com', crypt('demo1234', gen_salt('bf')), 1, 1, '+6281398765432');

-- 5. User-role mapping
INSERT INTO user_roles (user_id, role_id) VALUES
(1, 1),  -- admin    -> admin
(2, 3);  -- employee -> employee
