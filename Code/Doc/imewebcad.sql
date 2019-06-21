/*
Navicat MySQL Data Transfer

Source Server         : 本地
Source Server Version : 50711
Source Host           : localhost:3306
Source Database       : imewebcad

Target Server Type    : MYSQL
Target Server Version : 50711
File Encoding         : 65001

Date: 2019-06-20 16:40:33
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for t_cadfileshandlelog
-- ----------------------------
DROP TABLE IF EXISTS `t_cadfileshandlelog`;
CREATE TABLE `t_cadfileshandlelog` (
`ID`  int(11) NOT NULL AUTO_INCREMENT ,
`FileJavaID`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`FileCode`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`FTPPath`  varchar(555) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`FileName`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`UseCode`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`FileURL`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`State`  varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`Thumbnail`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`Referer`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`IP`  varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`IsCopy`  varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`UserAgent`  varchar(455) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`CreateTime`  datetime NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP ,
`Mark`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
PRIMARY KEY (`ID`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8 COLLATE=utf8_general_ci
AUTO_INCREMENT=89

;

-- ----------------------------
-- Records of t_cadfileshandlelog
-- ----------------------------
BEGIN;
INSERT INTO `t_cadfileshandlelog` VALUES ('84', 'imeTestCADFile_636966378084299478_cad_0', '4F6E640637A92637', 'F:/IMENETCADFTP/imeTestCADFile_636966378084299478_cad_0_.dwg', 'imeTestCADFile_636966378084299478_cad_0_.dwg', 'IME', 'E:/IMENetCadFiles/20190620//CAD/imeTestCADFile_636966378084299478_cad_0_.dwg', 'success', '', '', '', 'false', null, '2019-06-20 14:31:10', null), ('85', 'imeTestIMGFile_20190604_636966378331352234-pic_0', '7CBD979F6DB183FA', 'F:/IMENETCADFTP//imeTestIMGFile_20190604_636966378331352234-pic_0_.png', 'imeTestIMGFile_20190604_636966378331352234-pic_0_.png', 'IME', 'E:/IMENetCadFiles/20190620//CAD/imeTestIMGFile_20190604_636966378331352234-pic_0_.png', 'success', 'E:/IMENetCadFiles/20190620//Thumbnail/ThumbnailPic636966379882576014_300_300.jpg', '', '', 'false', null, '2019-06-20 14:33:08', null), ('86', 'imeTestPDFFile_20190604_636966378394624161_pdf_0', '639171600620FB03', 'F:/IMENETCADFTP//imeTestPDFFile_20190604_636966378394624161_pdf_0_.pdf', 'imeTestPDFFile_20190604_636966378394624161_pdf_0_.pdf', 'IME', 'E:/IMENetCadFiles/20190620//CAD/imeTestPDFFile_20190604_636966378394624161_pdf_0_.pdf', 'success', 'E:/IMENetCadFiles/20190620//Thumbnail/ThumbnailPDF636966379903096568_300_300.png', '', '', 'false', null, '2019-06-20 14:33:12', null), ('87', 'imeTestDXFFile_20190614_636966378475574426_dxf_0', 'CDF46C3BEEF0524D', 'F:/IMENETCADFTP/imeTestDXFFile_20190614_636966378475574426_dxf_0.dxf', 'imeTestDXFFile_20190614_636966378475574426_dxf_0.dxf', 'IME', 'E:/IMENetCadFiles/20190620//CAD/imeTestDXFFile_20190614_636966378475574426_dxf_0.dxf', 'success', 'E:/IMENetCadFiles//20190620/Thumbnail/Thumbnail636966379975961660_300_300.bmp', '', '', 'false', null, '2019-06-20 14:33:17', null), ('88', 'imeTestCADFile_636966379650710583_dwg_0', '29BA733635899643', 'F:/IMENETCADFTP/imeTestCADFile_636966379650710583_dwg_0_.dwg', 'imeTestCADFile_636966379650710583_dwg_0_.dwg', 'IME', 'E:/IMENetCadFiles/20190620//CAD/imeTestCADFile_636966379650710583_dwg_0_.dwg', 'success', 'E:/IMENetCadFiles/20190620//Thumbnail/Thumbnail636966379985857270_300_300.bmp', '', '', 'false', null, '2019-06-20 14:33:18', null);
COMMIT;

-- ----------------------------
-- Table structure for t_cadfilesreadinglog
-- ----------------------------
DROP TABLE IF EXISTS `t_cadfilesreadinglog`;
CREATE TABLE `t_cadfilesreadinglog` (
`ID`  int(11) NOT NULL AUTO_INCREMENT ,
`ReadingCode`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`HandleCode`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`CreateType`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '时效链接类别：View 查看大图，Download下载专用' ,
`InnerOrOut`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '' COMMENT '访问方式：In 内网访问，Out 外网访问，Both 内外网都有' ,
`FileURL`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`Thumbnail`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`CreaterCode`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`CreaterReferer`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`CreaterIP`  varchar(60) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`CreateTime`  datetime NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP ,
`ReaderReferer`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`ReaderIP`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`ReaderCode`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`LastReadingTime`  datetime NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP ,
`ReadingCount`  int(11) NULL DEFAULT NULL ,
`UserAgent`  varchar(455) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
PRIMARY KEY (`ID`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8 COLLATE=utf8_general_ci
AUTO_INCREMENT=4292

;

-- ----------------------------
-- Records of t_cadfilesreadinglog
-- ----------------------------
BEGIN;
INSERT INTO `t_cadfilesreadinglog` VALUES ('137', 'FAE322265385784FF4DBEB162BA925B7', 'testdwg123456', 'View', 'Both', 'E:\\IMENetCadFiles\\/20190430//CAD/test.dwg', 'E:\\IMENetCadFiles\\/20190430//Thumbnail/Thumbnail636922285792080610_300_300.bmp', 'JimiTest', '192.168.255.103|http://192.168.255.103:2019/', '192.168.88.33', '2019-05-15 12:11:46', '192.168.255.103|http://192.168.255.103:2019/', '192.168.88.33', 'Viewer', '2019-05-15 12:11:46', '1', 'Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36'), ('152', '47C962CC904B2981D7F46730F8DFC257', 'testdwg123456', 'View', 'Both', 'E:\\IMENetCadFiles\\/20190430//CAD/test.dwg', 'E:\\IMENetCadFiles\\/20190430//Thumbnail/Thumbnail636922285792080610_300_300.bmp', 'JimiTest', '192.168.255.103|http://192.168.255.103:2019/', '192.168.255.206', '2019-05-15 12:57:53', '192.168.255.103|http://192.168.255.103:2019/', '192.168.255.206', 'Viewer', '2019-05-15 12:57:53', '1', 'Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36'), ('156', 'A85E3E0753B11F3908C8A2192B6851', 'testdwg123456', 'View', 'Both', 'E:\\IMENetCadFiles\\/20190430//CAD/test.dwg', 'E:\\IMENetCadFiles\\/20190430//Thumbnail/Thumbnail636922285792080610_300_300.bmp', 'JimiTest', '192.168.255.103|http://192.168.255.103:2019/', '192.168.255.90', '2019-05-15 13:37:37', '192.168.255.103|http://192.168.255.103:2019/', '192.168.255.90', 'Viewer', '2019-05-15 13:37:37', '1', 'Mozilla/5.0 (Linux; Android 6.0; HLJS6 Build/MRA58K; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/66.0.3359.126 MQQBrowser/6.2 TBS/044611 Mobile Safari/537.36 MMWEBID/493 MicroMessenger/7.0.4.1420(0x2700043A) Process/tools NetType/WIFI Language/zh_CN'), ('174', 'B484C631BBDDA21CEE17D624CEFE7BF7', 'test', 'View', 'Both', 'E:\\IMENetCadFiles\\/20190425//CAD/test.png', 'E:\\IMENetCadFiles\\/20190425//Thumbnail/ThumbnailPic636917965245147759_300_300.jpg', 'JimiTest', '192.168.255.103|http://192.168.255.103:2019/', '192.168.2.66', '2019-05-28 10:37:13', '192.168.255.103|http://192.168.255.103:2019/', '192.168.2.66', 'Viewer', '2019-05-28 10:37:13', '1', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('175', '38458CD6AA68C47F64CF833C27245DEC', 'testpngs', 'View', 'Both', 'E:\\IMENetCadFiles\\/20190425//CAD/test.png', 'E:\\IMENetCadFiles\\/20190425//Thumbnail/ThumbnailPic636917966080067581_300_300.jpg', 'JimiTest', '192.168.255.103|http://192.168.255.103:2019/', '192.168.2.66', '2019-05-28 10:37:19', '192.168.255.103|http://192.168.255.103:2019/', '192.168.2.66', 'Viewer', '2019-05-28 10:37:19', '1', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('176', '98EAD1AC513FF02C88CE8B743B7DAA', 'testpdf123', 'View', 'Both', 'E:\\IMENetCadFiles\\/20190425//CAD/test.pdf', 'E:\\IMENetCadFiles\\/20190425//Thumbnail/ThumbnailPDF636918039614421035_300_300.png', 'JimiTest', '192.168.255.103|http://192.168.255.103:2019/', '192.168.2.66', '2019-05-28 10:37:28', '192.168.255.103|http://192.168.255.103:2019/', '192.168.2.66', 'Viewer', '2019-05-28 10:37:28', '1', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('177', '7F5D2C6E1F83DFDA127B6BDB6298647', 'testdwg123456', 'View', 'Both', 'E:\\IMENetCadFiles\\/20190430//CAD/test.dwg', 'E:\\IMENetCadFiles\\/20190430//Thumbnail/Thumbnail636922285792080610_300_300.bmp', 'JimiTest', '192.168.255.103|http://192.168.255.103:2019/', '192.168.2.66', '2019-05-28 10:37:36', '192.168.255.103|http://192.168.255.103:2019/', '192.168.2.66', 'Viewer', '2019-05-28 10:37:36', '1', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('178', '87EB8B7B3DFDFFF6D9C999289BD7DF1', 'test12334', 'View', 'Both', 'E:\\IMENetCadFiles\\/20190528//CAD/test.png', 'E:\\IMENetCadFiles\\/20190528//Thumbnail/ThumbnailPic636946363070302646_300_300.jpg', 'JimiTest', '192.168.255.103|http://192.168.255.103:2019/', '192.168.2.66', '2019-05-28 10:43:17', '192.168.255.103|http://192.168.255.103:2019/', '192.168.2.66', 'Viewer', '2019-05-28 10:43:17', '1', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('179', '5216B5381D8CA9FD7EEC5A829F45B', 'test', 'View', 'Both', 'E:\\IMENetCadFiles\\/20190425//CAD/test.png', 'E:\\IMENetCadFiles\\/20190425//Thumbnail/ThumbnailPic636917965245147759_300_300.jpg', 'JimiTest', '192.168.255.103|http://192.168.255.103:2019/List/HandleList', '192.168.2.66', '2019-05-28 10:50:14', '192.168.255.103|http://192.168.255.103:2019/List/HandleList', '192.168.2.66', 'Viewer', '2019-05-28 10:50:14', '1', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('180', 'A414F51ACEE4B99B14BB544D66FAADD3', '99999', 'View', 'Both', 'E:\\IMENetCadFiles\\/20190425//CAD/test.pdf', 'E:\\IMENetCadFiles\\/20190425//Thumbnail/ThumbnailPDF636918039614421035_300_300.png', 'JimiTest', '192.168.255.103|http://192.168.255.103:2019/List/HandleList', '192.168.2.66', '2019-05-28 10:50:40', '192.168.255.103|http://192.168.255.103:2019/List/HandleList', '192.168.2.66', 'Viewer', '2019-05-28 10:50:40', '1', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('181', 'C132447A7DA331EA928F3DF9F694030', '99999', 'View', 'Both', 'E:\\IMENetCadFiles\\/20190425//CAD/test.pdf', 'E:\\IMENetCadFiles\\/20190425//Thumbnail/ThumbnailPDF636918039614421035_300_300.png', 'JimiTest', '192.168.255.103|https://192.168.255.103:2020/', '192.168.2.66', '2019-05-28 10:56:59', '', '192.168.2.66', 'Viewer', '2019-05-28 10:56:59', '1', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('182', 'D5C71A904880975E79551D48E72A5D48', 'test', 'View', 'Both', 'E:\\IMENetCadFiles\\/20190425//CAD/test.png', 'E:\\IMENetCadFiles\\/20190425//Thumbnail/ThumbnailPic636917965245147759_300_300.jpg', 'JimiTest', '192.168.255.103|https://192.168.255.103:2020/', '192.168.2.66', '2019-05-28 11:09:35', '', '192.168.2.66', 'Viewer', '2019-05-28 11:09:35', '1', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('183', 'CA61881431FA897744217C5D51D5B56', 'test', 'View', 'Both', 'E:\\IMENetCadFiles\\/20190425//CAD/test.png', 'E:\\IMENetCadFiles\\/20190425//Thumbnail/ThumbnailPic636917965245147759_300_300.jpg', 'JimiTest', '192.168.255.103|https://192.168.255.103:2020/', '192.168.2.66', '2019-05-28 11:12:56', '', '192.168.2.66', 'Viewer', '2019-05-28 11:12:56', '1', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4260', 'ABE96584C2FD1251AB762A7EEA36B997', '79154A225F7CC632', 'ViewCAD', '', 'E:/IMENetCadFiles/20190531//CAD/imeTestFile_20190529_8_0_636949253874988169.dwg', '', '', '', '192.168.255.103', '2019-05-31 19:05:33', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4261', '598A5581AF70477412C85F788B3C041', '79154A225F7CC632', 'Download', '', 'E:/IMENetCadFiles/20190531//CAD/imeTestFile_20190529_8_0_636949253874988169.dwg', '', '', '', '192.168.255.103', '2019-05-31 19:06:12', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4262', '34F618BB29AE824E379AF6976FD41CA', '71D14047B84CD193', 'ViewIMG', '', 'E:/IMENetCadFiles/20190531//CAD/imeTestFileImg_20190529_8_0_636949253942203575.png', '', '', '192.168.255.103|http://192.168.255.103:2018/CADViewer/viewIMG/?code=vw152hyn1hma1b1q5cdodzeg8qmkpsm7&token=test', '192.168.255.103', '2019-05-31 19:07:19', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4263', '403BD3A7D15BABECAB5CD61F63CF5B17', '98F36ECCA122E1AD', 'ViewPDF', '', 'E:/IMENetCadFiles/20190531//CAD/imeTestFilePDF_20190529_8_0_636949254714193982.pdf', '', '', '192.168.255.103|http://192.168.255.103:2018/CADViewer/viewPDF/?code=vy1c2jyp1jnf2a2m5bdhdxdf8rmkqkn8&token=test', '192.168.255.103', '2019-05-31 19:11:21', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4264', '38C1843553FE351ECE9EE72ED67C590', '98F36ECCA122E1AD', 'Download', '', 'E:/IMENetCadFiles/20190531//CAD/imeTestFilePDF_20190529_8_0_636949254714193982.pdf', '', '', '', '192.168.255.103', '2019-05-31 19:11:51', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4265', 'F69BAF80B39EF2F5B2388F6345929DD', '3121F0A11FE3D610507A29EEE34CD924', 'ViewPDF', '', 'E:\\IMENetCadFiles\\/20190531//Copy/imeTestFilePDF_20190529_8_0_636949254714193982.pdf', '', '', '192.168.255.103|http://192.168.255.103:2018/CADViewer/viewPDF/?code=vs151fyn2jma281k4beme0dg8qmppkm47i7yj2nhjoys53noh0amgwnp8n4m4xdb&token=test', '192.168.255.103', '2019-05-31 19:16:09', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4266', '586BD231E46137EAE7B8B080B99A7FA', '3121F0A11FE3D610507A29EEE34CD924', 'Download', '', 'E:\\IMENetCadFiles\\/20190531//Copy/imeTestFilePDF_20190529_8_0_636949254714193982.pdf', '', '', '', '192.168.255.103', '2019-05-31 19:16:39', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4267', 'F76619AD7A1D3A445D5C251676CECD81', 'FD33CEDA9FEFF8FA5DAF122C6A5B27F1', 'ViewPDF', '', 'E:\\IMENetCadFiles\\/20190531//Copy/imeTestFilePDF_20190529_8_0_636949254714193982.pdf', '', '', '192.168.255.103|http://192.168.255.103:2018/CADViewer/viewPDF/?code=wv281gyp2gnf2b2k4jeme0ej8smrqpn57i82kwnmjnyl40nmg1bkgxno7l4k51d8&token=test', '192.168.255.103', '2019-05-31 19:26:30', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4268', '60F3674988178E29BA56C148F2A2B773', 'FD33CEDA9FEFF8FA5DAF122C6A5B27F1', 'Download', '', 'E:\\IMENetCadFiles\\/20190531//Copy/imeTestFilePDF_20190529_8_0_636949254714193982.pdf', '', '', '', '192.168.255.103', '2019-05-31 19:27:00', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4269', 'A0714F4DFFEE35F6141147AFB0CE42', 'B8CD6FEC44B2196C', 'ViewCAD', '', 'E:/IMENetCadFiles/20190603//CAD/imeTestFile_20190529_jio_8_636951833087081531.dwg', '', '', '', '192.168.255.103', '2019-06-04 09:15:18', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4270', '7AF6BEC5A87DE1D9AAC657F5D8F4216', '758AC15A5ED53C9E', 'ViewCAD', '', 'E:/IMENetCadFiles/20190604//CAD/imeTestCADFile_20190604_批量_4_636952430493369572.dwg', '', '', '192.168.255.103|https://192.168.255.103:2019/UnitTest/Index', '192.168.255.103', '2019-06-04 13:25:22', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4271', '15CAB295F238E2CB25324086D7BA21', '588735D1A083E42C', 'ViewPDF', '', 'E:/IMENetCadFiles/20190604//CAD/imeTestPDFFile_20190604_济淼_4_636952428205778918.pdf', '', '', 'localhost|http://localhost:56928//CADViewer/viewPDF/?code=vu1c1lyt1gmf2b1k5bdgd3dg8rmnpln7&token=test', '::1', '2019-06-04 13:28:04', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4272', '2FEF9FAC1F1B8BA31BA1747FDDB548B', '612747D8D9877B70', 'ViewCAD', '', 'E:/IMENetCadFiles/20190604//CAD/imeTestCADFile_20190604_中午测试_4_636952515592931308.dwg', '', '', '192.168.255.103|https://192.168.255.103:2019/UnitTest/Index', '192.168.255.103', '2019-06-04 13:29:54', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4273', 'EECD44D247FA478A4CDA6311D5CB9', '0BB161E5C0C45EEC', 'ViewIMG', '', 'E:/IMENetCadFiles/20190605//CAD/imeTestIMGFile_20190604__0_636953506068412766.png', '', '', 'localhost|http://localhost:56928//CADViewer/viewIMG/?code=vp262fyn1jmb2c1o5ddgeydh7rnoqon7&token=test', '::1', '2019-06-05 16:58:39', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4274', 'C6E320CB84E8BD638D232B47A44EDAEF', '0BB161E5C0C45EEC', 'ViewIMG', '', 'E:/IMENetCadFiles/20190605//CAD/imeTestIMGFile_20190604__0_636953506068412766.png', '', '', 'localhost|http://localhost:56928//CADViewer/viewIMG/?code=vp262fyn1jmb2c1o5ddgeydh7rnoqon7&token=test', '::1', '2019-06-05 17:00:54', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4275', '49EA94D9452D4716A946A3A3D6BAB067', '0BB161E5C0C45EEC', 'ViewIMG', '', 'E:/IMENetCadFiles/20190605//CAD/imeTestIMGFile_20190604__0_636953506068412766.png', '', '', 'localhost|http://localhost:56928//CADViewer/viewIMG/?code=vp262fyn1jmb2c1o5ddgeydh7rnoqon7&token=test', '::1', '2019-06-05 17:01:06', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4276', '9E4998B71A47A64C364D8E35F6517', '0BB161E5C0C45EEC', 'ViewIMG', '', 'E:/IMENetCadFiles/20190605//CAD/imeTestIMGFile_20190604__0_636953506068412766.png', '', '', 'localhost|http://localhost:56928//CADViewer/viewIMG/?code=vp262fyn1jmb2c1o5ddgeydh7rnoqon7&token=test', '::1', '2019-06-05 17:05:50', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4277', '9247127BE2EF63DE2D406CBFEDFCFB0', 'F81EC2A62363D85C', 'ViewPDF', '', 'E:/IMENetCadFiles/20190605//CAD/imeTestPDFFile_20190604__0_636953511595330188.pdf', '', '', 'localhost|http://localhost:56928//CADViewer/viewPDF/?code=wv1c1ezr2gmc281p4cdjd1dg8qmrpon7&token=test', '::1', '2019-06-05 17:06:19', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4278', '3BE5FD42F078666B2E66BB82BB1C5F6', '7EABDBD0E5A92ED8', 'ViewCAD', '', 'E:/IMENetCadFiles/20190606//CAD/imeTestCADFile_20190604_千_997_636954138250665622.dwg', '', '', 'localhost|https://localhost:2019/', '::1', '2019-06-06 11:06:22', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0'), ('4279', 'D6C277D5E0817C5976C2D382E749922', '7EABDBD0E5A92ED8', 'ViewCAD', '', 'E:/IMENetCadFiles/20190606//CAD/imeTestCADFile_20190604_千_997_636954138250665622.dwg', '', '', 'localhost|https://localhost:2019/', '::1', '2019-06-06 11:06:30', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0'), ('4280', 'F0519537AB938DAE24327BD12D921B68', 'E27E9DA36FB66FB8', 'ViewCAD', '', 'E:/IMENetCadFiles/20190606//CAD/imeTestCADFile_20190604_坏_995_636954269023089916.dwg', '', '', 'localhost|https://localhost:2019/', '::1', '2019-06-06 14:38:23', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0'), ('4281', '438687A557B5DCE53F6A681AEBA8D69A', 'E27E9DA36FB66FB8', 'ViewCAD', '', 'E:/IMENetCadFiles/20190606//CAD/imeTestCADFile_20190604_坏_995_636954269023089916.dwg', '', '', 'localhost|https://localhost:2019/', '::1', '2019-06-06 14:41:21', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0'), ('4282', '4F6DBB13EEBE5EA9018EA8257DA1D41', 'E27E9DA36FB66FB8', 'ViewCAD', '', 'E:/IMENetCadFiles/20190606//CAD/imeTestCADFile_20190604_坏_995_636954269023089916.dwg', '', '', 'localhost|https://localhost:2019/', '::1', '2019-06-06 14:41:50', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0'), ('4283', '7C3F69584B817C7D80A69F7297492A7', 'B810B768B8FF4CAA', 'ViewCAD', '', 'E:/IMENetCadFiles/20190606//CAD/imeTestCADFile_20190604_在_989_636954346510237928.dwg', '', '', 'localhost|https://localhost:2019/', '::1', '2019-06-06 17:17:59', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0'), ('4284', 'C4B23E46202F9219AE1A4F1F43878A', 'C902130AEB62DD20', 'ViewCAD', '', 'E:/IMENetCadFiles/20190612//CAD/imeTestCADFile_20190604__0_636959278483858542.dwg', '', '', 'localhost|http://localhost:56928/', '::1', '2019-06-12 09:21:34', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4285', '58DF1BBDDFD2131BAF333A4534CDF6A', 'C902130AEB62DD20', 'ViewCAD', '', 'E:/IMENetCadFiles/20190612//CAD/imeTestCADFile_20190604__0_636959278483858542.dwg', '', '', 'localhost|http://localhost:2018/', '::1', '2019-06-12 09:29:51', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:67.0) Gecko/20100101 Firefox/67.0'), ('4286', 'B9BC25A75EE31C1C0DA70B598DC2E20', 'C902130AEB62DD20', 'ViewCAD', '', 'E:/IMENetCadFiles/20190612//CAD/imeTestCADFile_20190604__0_636959278483858542.dwg', '', '', 'localhost|http://localhost:56928/', '::1', '2019-06-13 13:30:27', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4287', '197DBF11B9F72FC11730E58D4CAC5C6C', 'C902130AEB62DD20', 'ViewCAD', '', 'E:/IMENetCadFiles/20190612//CAD/imeTestCADFile_20190604__0_636959278483858542.dwg', '', '', 'localhost|http://localhost:56928/', '::1', '2019-06-13 14:03:15', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4288', '9740CFFBD89F9391512F18ED9DDB512A', 'C902130AEB62DD20', 'ViewCAD', '', 'E:/IMENetCadFiles/20190612//CAD/imeTestCADFile_20190604__0_636959278483858542.dwg', '', '', 'localhost|http://localhost:56928/', '::1', '2019-06-13 15:09:26', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4289', 'BCCF7A16348F9335AE9E01E1F780AA', 'EF7E5D811F6276CF', 'ViewCAD', '', 'E:/IMENetCadFiles/20190617//CAD/imeTestCADFile_20190604_c_1_636963619314452866.dwg', '', '', 'localhost|http://localhost:56928/', '::1', '2019-06-19 11:25:52', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4290', 'B7A0CC4D872E20CB7690EF1FA14553', 'BE1607CA97819A93', 'ViewCAD', '', 'E:/IMENetCadFiles/20190619//CAD/imeTestCADFile_636965356250395954__0_.dwg', '', '', 'localhost|http://localhost:56928/', '::1', '2019-06-19 11:52:12', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36'), ('4291', '2F961F9055CCE078FE7686316F2D68', '4F6E640637A92637', 'ViewCAD', '', 'E:/IMENetCadFiles/20190620//CAD/imeTestCADFile_636966378084299478_cad_0_.dwg', '', '', 'localhost|http://localhost:56928/', '::1', '2019-06-20 14:33:12', null, null, null, null, null, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36');
COMMIT;

-- ----------------------------
-- Table structure for t_user
-- ----------------------------
DROP TABLE IF EXISTS `t_user`;
CREATE TABLE `t_user` (
`ID`  int(11) NOT NULL AUTO_INCREMENT COMMENT 'ID' ,
`Code`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`UserName`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`Password`  varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
PRIMARY KEY (`ID`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8 COLLATE=utf8_general_ci
AUTO_INCREMENT=3

;

-- ----------------------------
-- Records of t_user
-- ----------------------------
BEGIN;
INSERT INTO `t_user` VALUES ('1', 'key001', 'Jimi', 'jimi'), ('2', 'key002', 'JimiTest', 'JimiTest');
COMMIT;

-- ----------------------------
-- Procedure structure for proc_addCADFileHandleLog
-- ----------------------------
DROP PROCEDURE IF EXISTS `proc_addCADFileHandleLog`;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `proc_addCADFileHandleLog`(in inFileJavaID VARCHAR(255), 
in FileCode VARCHAR(255), 
in FTPPath VARCHAR(255), 
in FileName VARCHAR(255), 
in UseCode VARCHAR(255), 
in FileURL VARCHAR(255), 
in State VARCHAR(255), 
in Thumbnail VARCHAR(255), 
in Referer VARCHAR(255), 
in IP VARCHAR(255), 
in isCopy VARCHAR(255))
BEGIN
    declare cnt int default 0;
    select count(*) into cnt from  t_cadfilesHandleLog where `FileJavaID`=inFileJavaID ;
    if cnt<1 THEN 
        INSERT INTO t_cadfilesHandleLog ( `FileJavaID`, `FileCode`, `FTPPath`, `FileName`, `UseCode`, `FileURL`, `State`, `Thumbnail`, `Referer`, `IP`, `CreateTime`,`isCopy`)
        VALUES (inFileJavaID, FileCode,FTPPath, FileName, UseCode, FileURL, State, Thumbnail, Referer, IP, NOW(),isCopy);
        select 'done';
    ELSE 
         select '重复记录';
    END if;
END
;;
DELIMITER ;

-- ----------------------------
-- Procedure structure for proc_pPage
-- ----------------------------
DROP PROCEDURE IF EXISTS `proc_pPage`;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `proc_pPage`(in _pagecurrent int, /*当前页*/  
in _pagesize int,        /*每页的记录数*/  
in _ifelse varchar(1000),/*显示字段*/  
in _where varchar(1000), /*条件*/  
in _order varchar(1000))
    COMMENT '分页存储过程\r\n调用例1  call proc_pPage(1,3,''*'',''test'',''order by id desc'');\r\n'
BEGIN  
if _pagesize<=1 then  
  set _pagesize=20;  
end if;  
if _pagecurrent < 1 then  
  set _pagecurrent = 1;  
end if;  
   
set @strsql = concat('select ',_ifelse,' from ',_where,' ',_order,' limit ',_pagecurrent*_pagesize-_pagesize,',',_pagesize);  
prepare stmtsql from @strsql;  
execute stmtsql;  
deallocate prepare stmtsql;   
  
set @strsqlcount=concat('select count(0) AS count from ',_where);/*count(1) 这个字段最好是主键*/  
prepare stmtsqlcount from @strsqlcount;  
execute stmtsqlcount;  
deallocate prepare stmtsqlcount;  
  
END
;;
DELIMITER ;

-- ----------------------------
-- Auto increment value for t_cadfileshandlelog
-- ----------------------------
ALTER TABLE `t_cadfileshandlelog` AUTO_INCREMENT=89;

-- ----------------------------
-- Auto increment value for t_cadfilesreadinglog
-- ----------------------------
ALTER TABLE `t_cadfilesreadinglog` AUTO_INCREMENT=4292;

-- ----------------------------
-- Auto increment value for t_user
-- ----------------------------
ALTER TABLE `t_user` AUTO_INCREMENT=3;
