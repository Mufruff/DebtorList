-- --------------------------------------------------------
-- Хост:                         127.0.0.1
-- Версия сервера:               12.3.3-MariaDB - MariaDB Server
-- Операционная система:         Win64
-- HeidiSQL Версия:              12.21.0.7344
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Дамп структуры базы данных debtors_db
CREATE DATABASE IF NOT EXISTS `debtors_db` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci */;
USE `debtors_db`;

-- Дамп структуры для таблица debtors_db.config
CREATE TABLE IF NOT EXISTS `config` (
  `id` int(11) NOT NULL DEFAULT 1,
  `password` varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Дамп данных таблицы debtors_db.config: ~1 rows (приблизительно)
INSERT INTO `config` (`id`, `password`) VALUES
	(1, '1234');

-- Дамп структуры для таблица debtors_db.debtors
CREATE TABLE IF NOT EXISTS `debtors` (
  `id` char(36) NOT NULL,
  `full_name` varchar(200) NOT NULL,
  `current_debt` decimal(12,2) NOT NULL DEFAULT 0.00,
  `interest_rate` decimal(5,2) NOT NULL DEFAULT 0.00,
  `date_issued` datetime NOT NULL,
  `due_date` datetime NOT NULL,
  `last_interest_date` datetime NOT NULL,
  `is_paid` tinyint(1) NOT NULL DEFAULT 0,
  `marked_for_deletion` tinyint(1) NOT NULL DEFAULT 0,
  `notes` text DEFAULT NULL,
  `original_debt` decimal(12,2) NOT NULL DEFAULT 0.00,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Дамп данных таблицы debtors_db.debtors: ~0 rows (приблизительно)
INSERT INTO `debtors` (`id`, `full_name`, `current_debt`, `interest_rate`, `date_issued`, `due_date`, `last_interest_date`, `is_paid`, `marked_for_deletion`, `notes`, `original_debt`) VALUES
	('43170d8b-fb3d-4f33-85fc-aab3566d570e', 'ffff', 11766.67, 5.00, '2026-05-01 00:00:00', '2026-06-01 00:00:00', '2026-09-15 04:11:12', 0, 0, '', 10000.00);

-- Дамп структуры для таблица debtors_db.history
CREATE TABLE IF NOT EXISTS `history` (
  `id` char(36) NOT NULL,
  `debtor_id` char(36) NOT NULL,
  `amount` decimal(12,2) NOT NULL,
  `change_date` datetime NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_history_debtor` (`debtor_id`),
  CONSTRAINT `fk_history_debtor` FOREIGN KEY (`debtor_id`) REFERENCES `debtors` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Дамп данных таблицы debtors_db.history: ~0 rows (приблизительно)
INSERT INTO `history` (`id`, `debtor_id`, `amount`, `change_date`) VALUES
	('cd458efe-7b27-457a-8aac-c359ddda6fda', '43170d8b-fb3d-4f33-85fc-aab3566d570e', 10000.00, '2026-09-15 04:10:59'),
	('f7844146-3d59-4a1e-9673-ae2707541fe1', '43170d8b-fb3d-4f33-85fc-aab3566d570e', 11766.67, '2026-09-15 04:11:12');

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
