# Laravel Legacy Scaffold

Initially generated from `symposium-laravel/app/Models` fillable fields. Confirmed
foreign keys, long-text mappings, normalized email constraints, and an initial
PostgreSQL migration have since been added. See `src/Symposium.Storage/Migrations/README.md`
for verified relationships and intentionally unresolved legacy mappings.

## Generated

- `ArticleInfo` -> table `article_info` route `/article-info`
- `ArticleToArticleType` -> table `article_to_article_type` route `/article-to-article-type`
- `ArticleToAuthor` -> table `article_to_author` route `/article-to-author`
- `ArticleToCitation` -> table `article_to_citation` route `/article-to-citation`
- `ArticleToFiles` -> table `article_to_files` route `/article-to-files`
- `ArticleToKeyword` -> table `article_to_keyword` route `/article-to-keyword`
- `ArticleToSubject` -> table `article_to_subject` route `/article-to-subject`
- `ArticleTypes` -> table `article_types` route `/article-types`
- `Articles` -> table `articles` route `/articles`
- `Authors` -> table `authors` route `/authors`
- `Citations` -> table `citations` route `/citations`
- `Countries` -> table `countries` route `/countries`
- `Dosyalar` -> table `dosyalar` route `/dosyalar`
- `Feedback` -> table `feedbacks` route `/feedback`
- `Feedbackdata` -> table `feedbackdata` route `/feedbackdata`
- `Gorevfiles` -> table `gorev_files` route `/gorevfiles`
- `Gorevicerik` -> table `gorev_icerik` route `/gorevicerik`
- `JournalInfo` -> table `journal_info` route `/journal-info`
- `JournalToArticleType` -> table `journal_to_article_type` route `/journal-to-article-type`
- `JournalToCountrie` -> table `journal_to_countrie` route `/journal-to-countrie`
- `JournalToPublisher` -> table `journal_to_publisher` route `/journal-to-publisher`
- `JournalToSubject` -> table `journal_to_subject` route `/journal-to-subject`
- `Journals` -> table `journals` route `/journals`
- `Keywords` -> table `keywords` route `/keywords`
- `Parameters` -> table `parameters` route `/parameters`
- `Presentations` -> table `presentations` route `/presentations`
- `Publishers` -> table `publishers` route `/publishers`
- `Publishs` -> table `publishs` route `/publishs`
- `Roles` -> table `roles` route `/roles`
- `Subjects` -> table `subjects` route `/subjects`
- `Topics` -> table `topics` route `/topics`
- `Types` -> table `types` route `/types`
- `VolumeToFiles` -> table `volume_to_files` route `/volume-to-files`
- `VolumeToYear` -> table `volume_to_year` route `/volume-to-year`
- `Volumes` -> table `volumes` route `/volumes`
- `Yorum` -> table `yorumlar` route `/yorum`

## Skipped

- `Ayarlar` table `users`: template table already exists
- `Files` table `files`: template table already exists
- `Kullanicilar` table `users`: template table already exists
- `Languages` table `languages`: template table already exists
