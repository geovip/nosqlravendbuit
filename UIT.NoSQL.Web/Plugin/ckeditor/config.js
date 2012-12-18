/*
Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function( config )
{
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';
	config.toolbar = 'MyToolbar';
 
	config.toolbar_MyToolbar =
	[
		{ name: 'styles', items : [ 'Font','FontSize' ] },
		{ name: 'basicstyles', items : [ 'Bold','Italic','Underline','Strike','-','RemoveFormat' ] },
		{ name: 'colors', items : [ 'TextColor','BGColor' ] },
		{ name: 'paragraph', items : [ 'NumberedList','BulletedList','-','Outdent','Indent','-','Blockquote' ] },
		{ name: 'links', items : [ 'Link','Unlink' ] },
		{ name: 'insert', items : [ 'Image','Table','HorizontalRule','PageBreak'] }

	];
};
