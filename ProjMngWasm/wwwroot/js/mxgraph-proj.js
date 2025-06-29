let gObj = [];

//let graph = null;
//let graphModel = null;
//let dotNetHelpers = new Map();

function GetGraObj(containerId) {
  var iniObj = null;
  for (var i = 0; i < gObj.length; i++) {
    iobj = gObj[i];
    if (iobj.containerId == containerId) {
      iniObj = iobj;
      break;
    }
  }
  return iniObj;
}



window.mxGraphInit = (containerId,  dotNetRef) => {
  var iniObj = GetGraObj(containerId);

  // 대상 만들어진것이 없으면 만든다.
  if (iniObj == null) {

    iniObj = {
      containerId: containerId,
      dotNetRef: dotNetRef,
      graph: null,
      graphModel: null
    };
    gObj.push(iniObj);

  }

  //dotNetHelpers.set(containerId, dotNetRef);

  const container = document.getElementById(containerId);
  if (!container) return;




  // Focused but invisible textarea during control or meta key events
  var textInput = document.createElement('textarea');



  mxEvent.disableContextMenu(container);
  iniObj.graph = new mxGraph(container);
  iniObj.graph.setConnectable(true);
  iniObj.graphModel = iniObj.graph.getModel();

  var graph = iniObj.graph;

  mxUtils.setOpacity(textInput, 0);
  textInput.style.width = '1px';
  textInput.style.height = '1px';
  var restoreFocus = false;
  var gs = iniObj.graph.gridSize;
  var lastPaste = null;
  var dx = 0;
  var dy = 0;


  iniObj.graphModel.beginUpdate();
  try {
    const parent = iniObj.graph.getDefaultParent();
    const v1 = iniObj.graph.insertVertex(parent, null, 'Customer', 100, 100, 100, 50);
    const v2 = iniObj.graph.insertVertex(parent, null, 'Order', 300, 100, 100, 50);
    const e1 = iniObj.graph.insertEdge(parent, null, '1:N', v1, v2);

    const edge = iniObj.graph.insertEdge(parent, null, '', v1, v2);
    edge.geometry.relative = true;
    edge.geometry.x = 0.5;
    edge.style = "endArrow=block;startLabel=1;endLabel=N;";


  }
  finally {
    iniObj.graphModel.endUpdate();
  }


  iniObj.graph.addListener(mxEvent.CLICK, function (sender, evt) {
    const cell = evt.getProperty('cell');
    if (cell != null && cell.vertex) {
      const label = cell.value;

      //const dotNetRef = dotNetHelpers.get(containerId);
      //if (dotNetRef) {
        iniObj.dotNetRef.invokeMethodAsync("OnNodeClicked", label);
      //} else {
      //  console.warn(`No dotNetHelper found for containerId: ${containerId}`);
      //}


    }
  });




  iniObj.graph.addListener(mxEvent.CLICK, function (sender, evt) {
    const cell = evt.getProperty('cell');
    if (cell != null && cell.vertex) {
      const label = cell.value;

      //const dotNetRef = dotNetHelpers.get(containerId);
      //if (dotNetRef) {
      iniObj.dotNetRef.invokeMethodAsync("OnNodeClicked", label);
      //} else {
      //  console.warn(`No dotNetHelper found for containerId: ${containerId}`);
      //}


    }
  });



  // 더블 클릭 시 텍스트 편집 팝업
  iniObj.graph.dblClick = function (evt, cell) {
    if (cell != null && cell.vertex) {
      const newName = prompt("엔터티 이름 수정:", cell.value);
      if (newName !== null && newName.trim() !== "") {
        iniObj.graph.getModel().beginUpdate();
        try {
          cell.value = newName;
          iniObj.graph.refresh();
        }
        finally {
          iniObj.graph.getModel().endUpdate();
        }
      }
    }
  };







  // Shows a textare when control/cmd is pressed to handle native clipboard actions
  mxEvent.addListener(document, 'keydown', function (evt) {
    // No dialog visible
    var source = mxEvent.getSource(evt);

    if (graph.isEnabled() && !graph.isMouseDown && !graph.isEditing() && source.nodeName != 'INPUT') {
      if (evt.keyCode == 224 /* FF */ || (!mxClient.IS_MAC && evt.keyCode == 17 /* Control */) ||
        (mxClient.IS_MAC && (evt.keyCode == 91 || evt.keyCode == 93) /* Left/Right Meta */)) {
        // Cannot use parentNode for check in IE
        if (!restoreFocus) {
          // Avoid autoscroll but allow handling of events
          textInput.style.position = 'absolute';
          textInput.style.left = (graph.container.scrollLeft + 10) + 'px';
          textInput.style.top = (graph.container.scrollTop + 10) + 'px';
          graph.container.appendChild(textInput);

          restoreFocus = true;
          textInput.focus();
          textInput.select();

          console.log('keydown', evt);

        }
      }
    }
  });

  // Restores focus on graph container and removes text input from DOM
  mxEvent.addListener(document, 'keyup', function (evt) {
    if (restoreFocus && (evt.keyCode == 224 /* FF */ || evt.keyCode == 17 /* Control */ ||
      evt.keyCode == 91 || evt.keyCode == 93 /* Meta */)) {
      restoreFocus = false;

      if (!graph.isEditing()) {
        graph.container.focus();
      }

      textInput.parentNode.removeChild(textInput);
      console.log('keyup', evt);
    }
  });

  mxClipboard.cellsToString = function (cells) {
    var codec = new mxCodec();
    var model = new mxGraphModel();
    var parent = model.getChildAt(model.getRoot(), 0);

    for (var i = 0; i < cells.length; i++) {
      model.add(parent, cells[i]);
    }

    return mxUtils.getXml(codec.encode(model));
  };



  // Inserts the XML for the given cells into the text input for copy
  var copyCells = function (graph, cells) {
    if (cells.length > 0) {
      var clones = graph.cloneCells(cells);

      // Checks for orphaned relative children and makes absolute
      for (var i = 0; i < clones.length; i++) {
        var state = graph.view.getState(cells[i]);

        if (state != null) {
          var geo = graph.getCellGeometry(clones[i]);

          if (geo != null && geo.relative) {
            geo.relative = false;
            geo.x = state.x / state.view.scale - state.view.translate.x;
            geo.y = state.y / state.view.scale - state.view.translate.y;
          }
        }
      }


      var copyStr = mxClipboard.cellsToString(clones);
      //textInput.value = copyStr;//  mxClipboard.cellsToString(clones);


      navigator.clipboard.writeText(copyStr).then(function () {
        console.log('Text copied to clipboard');
      }).catch(function (error) {
        console.error('Could not copy text: ', error);
      });

    }

    textInput.select();
    lastPaste = copyStr;// textInput.value;
  };

  // Handles copy event by putting XML for current selection into text input
  mxEvent.addListener(textInput, 'copy', mxUtils.bind(this, function (evt) {
    if (graph.isEnabled() && !graph.isSelectionEmpty()) {
      copyCells(graph, mxUtils.sortCells(graph.model.getTopmostCells(graph.getSelectionCells())));
      dx = 0;
      dy = 0;
    }
    console.log('copy', evt);
  }));

  // Handles cut event by removing cells putting XML into text input
  mxEvent.addListener(textInput, 'cut', mxUtils.bind(this, function (evt) {
    if (graph.isEnabled() && !graph.isSelectionEmpty()) {
      copyCells(graph, graph.removeCells());
      dx = -gs;
      dy = -gs;
    }
  }));

  // Merges XML into existing graph and layers
  var importXml = function (xml, dx, dy) {
    dx = (dx != null) ? dx : 0;
    dy = (dy != null) ? dy : 0;
    var cells = []

    try {
      var doc = mxUtils.parseXml(xml);
      var node = doc.documentElement;

      if (node != null) {
        var model = new mxGraphModel();
        var codec = new mxCodec(node.ownerDocument);
        codec.decode(node, model);

        var childCount = model.getChildCount(model.getRoot());
        var targetChildCount = graph.model.getChildCount(graph.model.getRoot());

        // Merges existing layers and adds new layers
        graph.model.beginUpdate();
        try {
          for (var i = 0; i < childCount; i++) {
            var parent = model.getChildAt(model.getRoot(), i);

            // Adds cells to existing layers if not locked
            if (targetChildCount > i) {
              // Inserts into active layer if only one layer is being pasted
              var target = (childCount == 1) ? graph.getDefaultParent() : graph.model.getChildAt(graph.model.getRoot(), i);

              if (!graph.isCellLocked(target)) {
                var children = model.getChildren(parent);
                cells = cells.concat(graph.importCells(children, dx, dy, target));
              }
            }
            else {
              // Delta is non cascading, needs separate move for layers
              parent = graph.importCells([parent], 0, 0, graph.model.getRoot())[0];
              var children = graph.model.getChildren(parent);
              graph.moveCells(children, dx, dy);
              cells = cells.concat(children);
            }
          }
        }
        finally {
          graph.model.endUpdate();
        }
      }
    }
    catch (e) {
      alert(e);
      throw e;
    }

    return cells;
  };

  // Parses and inserts XML into graph
  var pasteText = function (text) {

    console.log('pasteText', text);
    var xml = mxUtils.trim(text);
    var x = graph.container.scrollLeft / graph.view.scale - graph.view.translate.x;
    var y = graph.container.scrollTop / graph.view.scale - graph.view.translate.y;

    if (xml.length > 0) {
      if (lastPaste != xml) {
        lastPaste = xml;
        dx = 0;
        dy = 0;
      }
      else {
        dx += gs;
        dy += gs;
      }

      // Standard paste via control-v
      if (xml.substring(0, 14) == '<mxGraphModel>') {
        graph.setSelectionCells(importXml(xml, dx, dy));
        graph.scrollCellToVisible(graph.getSelectionCell());
      }
    }
  };

  // Cross-browser function to fetch text from paste events
  var extractGraphModelFromEvent = function (evt) {
    var data = null;

    if (evt != null) {
      var provider = (evt.dataTransfer != null) ? evt.dataTransfer : evt.clipboardData;

      if (provider != null) {
        if (document.documentMode == 10 || document.documentMode == 11) {
          data = provider.getData('Text');
        }
        else {
          data = (mxUtils.indexOf(provider.types, 'text/html') >= 0) ? provider.getData('text/html') : null;

          if (mxUtils.indexOf(provider.types, 'text/plain' && (data == null || data.length == 0))) {
            data = provider.getData('text/plain');
          }
        }
      }
    }

    return data;
  };

  // Handles paste event by parsing and inserting XML
  mxEvent.addListener(textInput, 'paste', function (evt) {
    // Clears existing contents before paste - should not be needed
    // because all text is selected, but doesn't hurt since the
    // actual pasting of the new text is delayed in all cases.
    textInput.value = '';

    if (graph.isEnabled()) {
      var xml = extractGraphModelFromEvent(evt);

      if (xml != null && xml.length > 0) {
        pasteText(xml);
      }
      else {
        // Timeout for new value to appear
        window.setTimeout(mxUtils.bind(this, function () {
          //pasteText(textInput.value);
          console.log('Timeout for new value to appear');
        }), 0);
      }
    }

    textInput.select();
    console.log('paste', evt);
  });

  // Enables rubberband selection
  new mxRubberband(graph);

  /*


  // Gets the default parent for inserting new cells. This
  // is normally the first child of the root (ie. layer 0).
  var parent = graph.getDefaultParent();

  // Adds cells to the model in a single step
  graph.getModel().beginUpdate();
  try {
    var v1 = graph.insertVertex(parent, null, 'Hello,', 20, 20, 80, 30);
    var v2 = graph.insertVertex(parent, null, 'World!', 200, 150, 80, 30);
    var e1 = graph.insertEdge(parent, null, '', v1, v2);
  }
  finally {
    // Updates the display
    graph.getModel().endUpdate();
  }

  */


};


















































window.addEntityNode = (containerId) => {
  var iniObj = GetGraObj(containerId);
  if (!iniObj.graph) return;

  const parent = iniObj.graph.getDefaultParent();
  const x = Math.floor(Math.random() * 400);
  const y = Math.floor(Math.random() * 300);
  const w = 120;
  const h = 50;

  iniObj.graphModel.beginUpdate();
  try {
    iniObj.graph.insertVertex(parent, null, 'Entity', x, y, w, h);
  } finally {
    iniObj.graphModel.endUpdate();
  }
};

window.removeSelectedEntity = (containerId) => {
  var iniObj = GetGraObj(containerId);
  if (!iniObj.graph) return;
  const cells = iniObj.graph.getSelectionCells();
  if (cells && cells.length > 0) {
    iniObj.graph.removeCells(cells, true);
  }
};


/*
if (cell.vertex) {
    // 엔터티
} else if (cell.edge) {
    // 관계선
}
*/



window.saveDiagramXml = (containerId) => {
  var iniObj = GetGraObj(containerId);
  if (!iniObj.graph) return "";
  const encoder = new mxCodec();
  const node = encoder.encode(iniObj.graph.getModel());
  return mxUtils.getXml(node);
};

window.loadDiagramXml = (containerId, xml) => {
  var iniObj = GetGraObj(containerId);
  if (!iniObj.graph) return "";
  const doc = mxUtils.parseXml(xml);
  const codec = new mxCodec(doc);
  const model = codec.decode(doc.documentElement);

  iniObj.graph.setModel(model);
  iniObj.graphModel = iniObj.graph.getModel(); // 재설정
};

window.saveDiagramAsObjectJson = (containerId) => {
  var iniObj = GetGraObj(containerId);
  if (!iniObj.graph) return "";
  const json = { entities: [], relations: [] };
  const parent = iniObj.graph.getDefaultParent();

  iniObj.graph.getModel().beginUpdate();
  try {
    const cells = iniObj.graph.getChildCells(parent);
    for (let cell of cells) {
      if (cell.vertex) {
        const parts = cell.value.split('\n');
        json.entities.push({
          id: cell.id,
          desc: parts[1] || "", // Description
          name: parts[0],
          fields: [],//parts.slice(1),
          x: cell.geometry.x,
          y: cell.geometry.y,
          w: cell.geometry.width,
          h: cell.geometry.height
        });
      } else if (cell.edge) {
        if (cell.source != null && cell.target != null) {

          json.relations.push({
            from: cell.source.id,
            to: cell.target.id,
            label: cell.value || ""
          });

        }
      }
    }
  }
  finally {
    iniObj.graph.getModel().endUpdate();
  }

  return JSON.stringify(json, null, 2);
};


// 전역에 한글자 픽셀 길이 저장
let monoCharWidth = null;

function getMonoCharWidth(fontSize = 11, fontFamily = 'Play') {
  if (monoCharWidth !== null) return monoCharWidth;
  const canvas = document.createElement('canvas');
  const ctx = canvas.getContext('2d');
  ctx.font = `${fontSize}px ${fontFamily}`;
  // '가'는 한글, 'A'는 영문, 고정폭 폰트에서 둘 다 같은 폭
  monoCharWidth = ctx.measureText('h').width; // 한글로 가 ... 로 하는게 좋타.
  return monoCharWidth;
}

// label의 width 계산 (가장 긴 줄 기준)
function getLabelWidthByCharCount(label, padding = 0) {
  const lines = label.split('\n');
  let maxLen = 0;
  for (let line of lines) {
    if (line.length > maxLen) maxLen = line.length;
  }
  const charWidth = getMonoCharWidth();
  return Math.ceil(maxLen * charWidth + padding);
}



window.loadDiagramFromObjectJson = (containerId, jsonStr) => {
  var iniObj = GetGraObj(containerId);
  if (!iniObj.graph) return "";
  const json = JSON.parse(jsonStr);
  const parent = iniObj.graph.getDefaultParent();
  const nodes = {};

  iniObj.graphModel.beginUpdate();
  try {
    iniObj.graph.removeCells(iniObj.graph.getChildCells(parent, true, true)); // clear
    for (let entity of json.entities) {
      const label = entity.name + '\n' + entity.desc;// + '\n' + entity.fields.join('\n');
      //var ww = entity.w || 150;
      var ww = entity.w ||getLabelWidthByCharCount(label);
      var hh = entity.h || 40;
      const node = iniObj.graph.insertVertex(parent, entity.id, label, entity.x, entity.y, ww, hh);
      nodes[entity.id] = node;
    }

    for (let rel of json.relations) {
      iniObj.graph.insertEdge(parent, null, rel.label, nodes[rel.from], nodes[rel.to]);
    }
  }
  catch (ee) {
    console.error(ee);


    //const dotNetRef = dotNetHelpers.get(_containerId);
    //if (dotNetRef) {
      //dotNetRef.invokeMethodAsync("OnNodeClicked", label);
      iniObj.dotNetRef.invokeMethodAsync("OnError", "loadDiagramFromObjectJson", ee.message);
    //} else {
    //  console.warn(`No dotNetHelper found for containerId: ${_containerId}`);
    //a}


  }
  finally {
    iniObj.graphModel.endUpdate();
  }
};

window.autoLayout_xxxxx = (containerId) => {
  var iniObj = GetGraObj(containerId);
  if (!iniObj.graph) return "";
  const layout = new mxHierarchicalLayout(iniObj.graph);
  layout.execute(iniObj.graph.getDefaultParent());
};


window.autoLayout = (containerId, gap = 5, maxWidth = 1800) => {

  var iniObj = GetGraObj(containerId);
  if (!iniObj.graph) return "";

  const parent = iniObj.graph.getDefaultParent();
  const cells = iniObj.graph.getChildVertices(parent);

  let x = gap;
  let y = gap;
  let rowMaxHeight = 0;

  iniObj.graph.getModel().beginUpdate();
  try {
    for (let cell of cells) {
      const geo = cell.geometry;
      if (!geo) continue;

      // 줄바꿈 필요하면 y 증가, x 초기화
      if (x + geo.width + gap > maxWidth) {
        x = gap;
        y += rowMaxHeight + gap;
        rowMaxHeight = 0;
      }

      geo.x = x;
      geo.y = y;

      x += geo.width + gap;
      if (geo.height > rowMaxHeight) rowMaxHeight = geo.height;
    }
  } finally {
    iniObj.graph.getModel().endUpdate();
    iniObj.graph.refresh(); // 이 부분이 중요!
  }
};



window.saveDiagramAsXml = (containerId) => {
  var xml = document.getElementById(containerId).contentWindow.getXml();
  return xml;
};



window.loadDiagramFromXml = (containerId, xml) => {
  document.getElementById(containerId).contentWindow.loadXml(xml);
};



const test = {
  "entities": [
    {
      "id": "1",
      "name": "Customer",
      "fields": ["Id", "Name", "Email"],
      "x": 100,
      "y": 100
    },
    {
      "id": "2",
      "name": "Order",
      "fields": ["Id", "CustomerId", "Amount"],
      "x": 300,
      "y": 100
    }
  ],
  "relations": [
    {
      "from": "1",
      "to": "2",
      "label": "1:N"
    }
  ]
}


window.initMxGraphEditor = (containerId) => {
  mxBasePath = 'mxgraph'; // mxClient.js 기준 경로
  if (!mxClient.isBrowserSupported()) {
    alert('Browser is not supported!');
    return;
  }


  const container = document.getElementById(containerId);


  // 에디터 초기화
  const editor = new mxEditor();
  editor.setGraphContainer(container);

  // 툴바 생성 및 붙이기
  //const toolbar = new mxDefaultToolbar(document.getElementById('toolbar'), editor);
  //editor.toolbar = toolbar;


  // 간단히 셀 하나 추가
  const parent = editor.graph.getDefaultParent();
  editor.graph.getModel().beginUpdate();
  try {
    editor.graph.insertVertex(parent, null, 'Node', 20, 20, 80, 30);
  } finally {
    editor.graph.getModel().endUpdate();
  }

  /*
  return;

  // 기본 설정
  let container = document.getElementById(graphContainerId);
  let toolbarContainer = document.getElementById(toolbarId);
  let sidebarContainer = document.getElementById(sidebarId);


  // 그래프 생성
  let graph = new mxGraph(container);
  graph.setConnectable(true);
  new mxRubberband(graph);

  // 기본 셀 하나 넣어보기
  let parent = graph.getDefaultParent();
  graph.getModel().beginUpdate();
  try {
    let v1 = graph.insertVertex(parent, null, 'Hello', 20, 20, 80, 30);
  } finally {
    graph.getModel().endUpdate();
  }


  // 툴바 구성
  let toolbar = new mxToolbar(toolbarContainer);
  toolbar.enabled = true;

  const addVertex = (icon, width, height, style) => {
    let vertex = new mxCell(null, new mxGeometry(0, 0, width, height), style);
    vertex.setVertex(true);

    let img = addToolbarItem(graph, toolbar, vertex, icon);
    img.style.cursor = 'pointer';
  };

  const addToolbarItem = (graph, toolbar, prototype, image) => {
    let funct = (graph, evt, cell) => {
      graph.stopEditing(false);
      let pt = graph.getPointForEvent(evt);
      let vertex = graph.getModel().cloneCell(prototype);
      vertex.geometry.x = pt.x;
      vertex.geometry.y = pt.y;
      graph.getModel().beginUpdate();
      try {
        graph.addCell(vertex);
      } finally {
        graph.getModel().endUpdate();
      }
    };

    return toolbar.addMode(null, image, () => {
      let handler = (evt) => {
        funct(graph, evt);
      };
      graph.container.addEventListener('mousedown', handler, { once: true });
    });
  };

  // 사이드바 구성
  const createSidebarEntry = (title, icon, width, height, style) => {
    let div = document.createElement('div');
    div.style.padding = '4px';
    div.style.cursor = 'pointer';
    div.style.display = 'flex';
    div.style.alignItems = 'center';
    div.innerHTML = `<img src="${icon}" style="width:20px;height:20px;margin-right:5px;"> ${title}`;
    div.onclick = () => {
      addVertex(icon, width, height, style);
    };
    sidebarContainer.appendChild(div);
  };

  // 샘플 도형 추가
  createSidebarEntry('Rectangle', 'https://jgraph.github.io/mxgraph/images/rectangle.gif', 100, 40, 'shape=rectangle;fillColor=#FFFFFF;strokeColor=#000000');
  createSidebarEntry('Ellipse', 'https://jgraph.github.io/mxgraph/images/ellipse.gif', 80, 80, 'shape=ellipse;fillColor=#FFFFFF;strokeColor=#000000');






  */


};


window.dynamicScriptLoader = {
  load: function (url) {
    return new Promise((resolve, reject) => {
      if (document.querySelector(`script[src="${url}"]`)) {
        resolve(); // 이미 로드됨
        return;
      }

      const script = document.createElement("script");
      script.src = url;
      script.type = "text/javascript";
      script.onload = resolve;
      script.onerror = () => reject(`Failed to load ${url}`);
      document.head.appendChild(script);
    });
  }
};