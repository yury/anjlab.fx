/*
This table contains list of russian personal ID types (in Russian)
Author: Alex Zakharov
Date: 2/1/2008
*/

if not exists (select * from sysobjects where id = object_id(N'fx.IdentityCardTypes') and xtype = 'U' )
begin

CREATE TABLE fx.IdentityCardTypes (
	IdentityCardCode tinyint not null,
	CardName nvarchar(512) not null,
	CardNotes nvarchar(max) null,
	CONSTRAINT PK_IdentityCardTypes PRIMARY KEY CLUSTERED (IdentityCardCode ASC)
)

DELETE FROM fx.IdentityCardTypes
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	1, N'Паспорт гражданина СССР', N'Действителен до 01.01.2006 для иностранных граждан и лиц без гражданства в соответствии с Постановлением Правительства Российской Федерации от 04.12.2003 N 731')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	3, N'Свидетельство о рождении', N'Для лиц, не достигших 16-летнего (с 01.10.97 - 14-летнего) возраста, оформленное в соответствии с законодательством Российской Федерации')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	4, N'Удостоверение личности офицера', N'Для военнослужащих (офицеров, прапорщиков, мичманов)')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	5, N'Справка об освобождении из места лишения свободы', N'Для лиц, освободившихся из мест лишения свободы')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	6, N'Паспорт Минморфлота', N'Паспорт моряка Минморфлота СССР (Российской Федерации), выданный до 1997 г.')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	7, N'Военный билет солдата (матроса, сержанта, старшины)', N'Военный билет для солдат, матросов, сержантов и старшин, проходящих военную службу по призыву или контракту')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	8, N'Временное удостоверение, выданное взамен военного билета', N'Документ, заменяющий паспорт гражданина (для лиц, которые проходят военную службу) в соотв. со ст. 2 Федеральный закон от 12.06.2002 N 67-ФЗ')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	9, N'Дипломатический паспорт гражданина Российской Федерации', N'Дипломатический паспорт для граждан Российской Федерации')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	10, N'Иностранный паспорт', N'Заграничный паспорт для постоянно проживающих за границей физических лиц, которые временно находятся на территории Российской Федерации')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	11, N'Свидетельство о рассмотрении ходатайства о признании беженцем на территории Российской Федерации по существу', N'Для лиц, ходатайствующих о признании беженцем (выдается на срок рассмотрения ходатайства, но не более 3 месяцев), Постановление Правительства Российской Федерации от 28.05.1998 N 523')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	12, N'Вид на жительство лица без гражданства', N'Вид на жительство в Российской Федерации для лиц без гражданства в соответствии со статьями 2 и 10 Федерального закона от 25.07.2002 N 115-ФЗ')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	13, N'Удостоверение беженца в Российской Федерации', N'Для лиц (не граждан Российской Федерации), признанных беженцами в соответствии со ст. 1 и 7 ФЗ от 19.02.1993 N 4528-1')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	14, N'Временное удостоверение личности гражданина Российской Федерации', N'Временное удостоверение личности гражданина Российской Федерации по форме 2П')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	15, N'Разрешение на временное проживание лица без гражданства в Российской Федерации', N'Разрешение на временное проживание лица без гражданства в Российской Федерации (для лиц, не имеющих документа, удостоверяющего личность)')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	18, N'Свидетельство о предоставлении временного убежища на территории Российской Федерации', N'Для иностранных граждан и лиц без гражданства, получивших временное убежище в соответствии со статьей 12 Федерального закона от 19.02.1993 N 4528-1')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	21, N'Паспорт гражданина Российской Федерации', N'Паспорт гражданина Российской Федерации, действующий на территории Российской Федерации с 1 октября 1997 года')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	22, N'Загранпаспорт гражданина Российской Федерации', N'Паспорт, удостоверяющий личность гражданина Российской Федерации за пределами Российской Федерации, образца 1997 года')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	23, N'Свидетельство о рождении, выданное уполномоченным органом иностранного государства', N'Для иностранных граждан, не достигших 16-летнего возраста')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	24, N'Удостоверение личности военнослужащего Российской Федерации', N'Заменяет с 2004 года удостоверение личности офицера (прапорщика, мичмана) в соответствии с Постановлением Правительства Российской Федерации от 12.02.2003 N 91')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	26, N'Паспорт моряка', N'Паспорт моряка (удостоверение личности гражданина, работающего на судах заграничного плавания или на иностранных судах) образца 1997 года	 ')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	27, N'Военный билет офицера запаса', N'Военный билет офицера запаса')
INSERT INTO fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) VALUES(	91, N'Иные документы, предусмотренные законодательством Российской Федерации', N'Иные документы, предусмотренные законодательством Российской Федерации или международными договорами в качестве документов, удостоверяющих личность')

end
go