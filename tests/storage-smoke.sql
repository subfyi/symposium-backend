\set ON_ERROR_STOP on
BEGIN;
DO $$
DECLARE
    article_id integer;
BEGIN
    INSERT INTO users (email, password, type, created_at)
        VALUES (' Schema-Smoke@Example.Test ', 'test-only', 1, now());
    BEGIN
        INSERT INTO users (email, password, type, created_at)
            VALUES ('schema-smoke@example.test', 'test-only', 1, now());
        RAISE EXCEPTION 'Normalized email uniqueness is missing';
    EXCEPTION WHEN unique_violation THEN NULL;
    END;
    BEGIN
        INSERT INTO users (email, password, type, created_at)
            VALUES ('   ', 'test-only', 1, now());
        RAISE EXCEPTION 'Blank email constraint is missing';
    EXCEPTION WHEN check_violation THEN NULL;
    END;
    INSERT INTO articles (en_title, en_abstract)
        VALUES (repeat('Long title ', 100), repeat('Long abstract ', 1000)) RETURNING id INTO article_id;
    IF (SELECT length(en_abstract) FROM articles WHERE id = article_id) < 10000 THEN
        RAISE EXCEPTION 'Long text was truncated';
    END IF;
    INSERT INTO authors (p_id, first_name) VALUES (article_id, 'Schema smoke');
    BEGIN
        DELETE FROM articles WHERE id = article_id;
        RAISE EXCEPTION 'Restrictive parent delete constraint is missing';
    EXCEPTION WHEN foreign_key_violation OR restrict_violation THEN NULL;
    END;
    BEGIN
        INSERT INTO authors (p_id) VALUES (2147483647);
        RAISE EXCEPTION 'Author foreign key is missing';
    EXCEPTION WHEN foreign_key_violation THEN NULL;
    END;
    BEGIN
        INSERT INTO files (folder_id, name, size, content_type, access_url, created_at)
            VALUES ('test', 'test.pdf', -1, 'application/pdf', '/test.pdf', now());
        RAISE EXCEPTION 'File size constraint is missing';
    EXCEPTION WHEN check_violation THEN NULL;
    END;
END $$;
ROLLBACK;
