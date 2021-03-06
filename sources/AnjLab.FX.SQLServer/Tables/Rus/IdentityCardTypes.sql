/*
<summary>
	Russian personal ID types dictionary (in Russian)
<summary>

<author>
	Alex Zakharov
	Copyright © AnjLab 2008, http://anjlab.com. All rights reserved.
	The code can be used for free as long as this copyright notice is not removed.
<author>

<date>1/2/2008</date>
*/

set nocount on

if not exists (select * from sysobjects where id = object_id(N'fx.IdentityCardTypes') and xtype = 'U' )
begin

create table fx.IdentityCardTypes (
	IdentityCardCode tinyint not null,
	CardName nvarchar(512) not null,
	CardNotes nvarchar(max) null,
	IsFavorite bit not null constraint dfIdentityCardTypesIsFavorite default 0,
	constraint pkIdentityCardTypes primary key clustered (IdentityCardCode asc)
)

exec sys.sp_addextendedproperty @name=N'MS_Description', 
	@value=N'Russian Classification of identiry card types', 
	@level0type=N'SCHEMA', @level0name=N'fx', @level1type=N'TABLE',  @level1name=N'IdentityCardTypes'

exec sys.sp_addextendedproperty @name=N'MS_Description', 
	@value=N'Official document name' , 
	@level0type=N'SCHEMA', @level0name=N'fx', @level1type=N'TABLE',  @level1name=N'IdentityCardTypes', @level2type=N'COLUMN',  @level2name=N'CardName'

exec sys.sp_addextendedproperty @name=N'MS_Description', 
	@value=N'Explanation regarding to using document type.' , 
	@level0type=N'SCHEMA', @level0name=N'fx', @level1type=N'TABLE',  @level1name=N'IdentityCardTypes', @level2type=N'COLUMN',  @level2name=N'CardNotes'

exec sys.sp_addextendedproperty @name=N'MS_Description', 
	@value=N'You can use this flag to define ID card types which are used often (for example, to truncate full list)' , 
	@level0type=N'SCHEMA', @level0name=N'fx', @level1type=N'TABLE',  @level1name=N'IdentityCardTypes', @level2type=N'COLUMN',  @level2name=N'IsFavorite'

insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	1, N'Паспорт гражданина СССР', N'Действителен до 01.01.2006 для иностранных граждан и лиц без гражданства в соответствии с Постановлением Правительства Российской Федерации от 04.12.2003 N 731')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	3, N'Свидетельство о рождении', N'Для лиц, не достигших 16-летнего (с 01.10.97 - 14-летнего) возраста, оформленное в соответствии с законодательством Российской Федерации')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	4, N'Удостоверение личности офицера', N'Для военнослужащих (офицеров, прапорщиков, мичманов)')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	5, N'Справка об освобождении из места лишения свободы', N'Для лиц, освободившихся из мест лишения свободы')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	6, N'Паспорт Минморфлота', N'Паспорт моряка Минморфлота СССР (Российской Федерации), выданный до 1997 г.')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	7, N'Военный билет солдата (матроса, сержанта, старшины)', N'Военный билет для солдат, матросов, сержантов и старшин, проходящих военную службу по призыву или контракту')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	8, N'Временное удостоверение, выданное взамен военного билета', N'Документ, заменяющий паспорт гражданина (для лиц, которые проходят военную службу) в соотв. со ст. 2 Федеральный закон от 12.06.2002 N 67-ФЗ')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	9, N'Дипломатический паспорт гражданина Российской Федерации', N'Дипломатический паспорт для граждан Российской Федерации')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	10, N'Иностранный паспорт', N'Заграничный паспорт для постоянно проживающих за границей физических лиц, которые временно находятся на территории Российской Федерации')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	11, N'Свидетельство о рассмотрении ходатайства о признании беженцем на территории Российской Федерации по существу', N'Для лиц, ходатайствующих о признании беженцем (выдается на срок рассмотрения ходатайства, но не более 3 месяцев), Постановление Правительства Российской Федерации от 28.05.1998 N 523')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	12, N'Вид на жительство лица без гражданства', N'Вид на жительство в Российской Федерации для лиц без гражданства в соответствии со статьями 2 и 10 Федерального закона от 25.07.2002 N 115-ФЗ')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	13, N'Удостоверение беженца в Российской Федерации', N'Для лиц (не граждан Российской Федерации), признанных беженцами в соответствии со ст. 1 и 7 ФЗ от 19.02.1993 N 4528-1')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	14, N'Временное удостоверение личности гражданина Российской Федерации', N'Временное удостоверение личности гражданина Российской Федерации по форме 2П')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	15, N'Разрешение на временное проживание лица без гражданства в Российской Федерации', N'Разрешение на временное проживание лица без гражданства в Российской Федерации (для лиц, не имеющих документа, удостоверяющего личность)')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	18, N'Свидетельство о предоставлении временного убежища на территории Российской Федерации', N'Для иностранных граждан и лиц без гражданства, получивших временное убежище в соответствии со статьей 12 Федерального закона от 19.02.1993 N 4528-1')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes, IsFavorite) values(	21, N'Паспорт гражданина Российской Федерации', N'Паспорт гражданина Российской Федерации, действующий на территории Российской Федерации с 1 октября 1997 года', 1)
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes, IsFavorite) values(	22, N'Загранпаспорт гражданина Российской Федерации', N'Паспорт, удостоверяющий личность гражданина Российской Федерации за пределами Российской Федерации, образца 1997 года', 1)
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	23, N'Свидетельство о рождении, выданное уполномоченным органом иностранного государства', N'Для иностранных граждан, не достигших 16-летнего возраста')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	24, N'Удостоверение личности военнослужащего Российской Федерации', N'Заменяет с 2004 года удостоверение личности офицера (прапорщика, мичмана) в соответствии с Постановлением Правительства Российской Федерации от 12.02.2003 N 91')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	26, N'Паспорт моряка', N'Паспорт моряка (удостоверение личности гражданина, работающего на судах заграничного плавания или на иностранных судах) образца 1997 года	 ')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	27, N'Военный билет офицера запаса', N'Военный билет офицера запаса')
insert into fx.IdentityCardTypes(IdentityCardCode, CardName, CardNotes) values(	91, N'Иные документы, предусмотренные законодательством Российской Федерации', N'Иные документы, предусмотренные законодательством Российской Федерации или международными договорами в качестве документов, удостоверяющих личность')

end
go