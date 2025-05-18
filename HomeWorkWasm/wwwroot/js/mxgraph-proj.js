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

window.mxGraphInit = (containerId, dotNetRef) => {
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

  mxEvent.disableContextMenu(container);
  iniObj.graph = new mxGraph(container);
  iniObj.graph.setConnectable(true);
  iniObj.graphModel = iniObj.graph.getModel();

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
      var ww = entity.w || 120;
      var hh = entity.h || 50;
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

window.autoLayout = (containerId) => {
  var iniObj = GetGraObj(containerId);
  if (!iniObj.graph) return "";
  const layout = new mxHierarchicalLayout(iniObj.graph);
  layout.execute(iniObj.graph.getDefaultParent());
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