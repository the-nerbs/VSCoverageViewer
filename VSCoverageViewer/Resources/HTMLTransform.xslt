<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl">

  <xsl:output method="html" indent="yes" />
  <xsl:param name="depth" select="3" />
  <xsl:param name="genDate" />
  <xsl:param name="totalLines" />
  <xsl:param name="totalBlocks" />

  <xsl:template match="/">
    <xsl:text disable-output-escaping="yes">&lt;!DOCTYPE html&gt;</xsl:text>
    <xsl:text disable-output-escaping="yes">&lt;!-- saved from url=(0016)http://localhost --&gt;</xsl:text>
    <html>
      <head>
        <title><xsl:value-of select="CovProj/Name" /> Coverage Summary</title>
        <script src="https://code.jquery.com/jquery-3.1.1.slim.min.js" />
        <style>
          table thead {
            background-color: #eee;
            color: #000000;
            font-weight: bold;
            cursor: default;
          }

          td div {
            display: inline-flex;
          }

          td.module-data {
            padding-left: 5px;
          }
          td.namespace-data {
            padding-left: 26px;
          }
          td.class-data {
            padding-left: 46px;
          }
          td.method-data {
            padding-left: 56px;
          }

          .type {
            height: 16px;
            white-space: nowrap;
            text-align: center;
            float: left;

            font-family: "Consolas", Monaco, monospace;
            font-size: 9pt;
          }
          .type.module {
            background-image: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsEAAA7BAbiRa+0AAAAadEVYdFNvZnR3YXJlAFBhaW50Lk5FVCB2My41LjExR/NCNwAAAWZJREFUOE+lkd0rQ2Ecx8//JCmu3EhISm5ILhhNbFHzumwyDeOYyGqsjcxCyNKaEhcaaduFtwu2ItPxUmcxtcM2vs45e/H2rJxcfHrq2+/z/T1PDwXgXxBDKRBDKRBDKRBDgck5FwzmNfRPONA9bIVSa4Kscxy1Sj0qmzQoq+vix7IU0BYnfwDxRALcSwxPkSge2AhCt2EEru5xen4D7/El8svlv2UB3dQSEvE3cNwrHp/TMotgSvbxcu/oInJLZKBCDMvt+wNYdx++pwt6RmyI8vLn5u/ynu8CKv08corrQXn8Qdg3DkDPusSzj14W35uRGUG+w0nq2oK87TlD24AlWbC65ePGeFltWMjcoKGDFuVrJpzZLMrepLy5c4QWjTlZkJa+Ut06KG42O3ZhtLoxZHJCa1zhf8OOdp0NCu0M5Orp7AUCRTUqFFYpUFDRjLzSRnH4J8IcUZYCMZQCMZQCMfw7oD4AbR3/0LPnzA0AAAAASUVORK5CYII=');
            width: 16px;
            margin-top: 1px;
            margin-right: 5px;
          }
          .type.namespace {
            background-image: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsEAAA7BAbiRa+0AAAIBSURBVDhPrVM9aJpRFH1DiGhERGJwcBUKpZBBgwUDGQoOhYIZRaQiODgUHAMSM7m5tLo4hEwGvqFIQBcL1kFDly7FLgY7KlKhKoI/n96c+3yJhkqhkAOP73vn3Hffffe8JzYxn8+Pl8vlZ4zb6XT6StFC1/V34L4R0ZfZbHak6KeAeFKpVMjlclEoFKLFYvGR+W63u9dqtXoOh4N8Ph+Nx+MeYm1y0Saw4MrtdpPZbKZ8Ps8JLphHsKHf7/8OBALEU03TQNEJa08Asul0OikWixHKvcHcqCSB47zEfGCxWCiVSnGCuJLW4AR2u53S6TTvfqboR0C/Y31rAj4nmvPHYDBQLpf7OwAAJyuMx+Osf1L0Cii5mEwm5RlrtRrBjTcrZQ1UdR2NRslqtVK9Xl/CmVMpDAYDW6PR0PFLHo+Hs/eq1eqOFDcA3g83yGQykd/v5z5pShJiMpn85NLwKytA016slDXY1nA4TEajkcrlMqGCt0oSYjgc7qNs2eVMJsNVhJT0COz4/cElJLtU9BpYJF3gLv+3C4yHBIlEggO0ZrO5qyQBhw5Rweif9wABX71eL/GVLRQKHHTOPL620Wg0jkQiskfZbHarS/xg3pdKJWlTMBjkBDnmsfig3W7r3H1+J51OZ6tLEjj7OcYdxg8kOFQ08x/g1C/sXMR4rejngBD3Cr6SUZK5AcQAAAAASUVORK5CYII=');
            width: 16px;
            margin-top: 1px;
            margin-right: 5px;
          }
          .type.class {
            background-image: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsEAAA7BAbiRa+0AAAAadEVYdFNvZnR3YXJlAFBhaW50Lk5FVCB2My41LjExR/NCNwAAATtJREFUOE+NjzEvBEEYhvcHiEqiOb9ARMF1Oj/g9DqicgmJRqGguKtcKe4ikWglGpxSriSiITknFLZQKNwddptVvOYd+SYzO7OheHZnd+d55tsIgMNVc3wyPh4D7/lvIZwHkdO4xsC/ImYhchLXkXRX8XGzEIwkjdR5duT0pYavbhWfSh50ZvHensDO0oiJUL5buYcdscauG3nYKSu5FJSzkwx2JJJ/Th5+xx6qk/sFJ1POmt94Wn/G7lRDR/QEh5ujSq7osQeX01pe29439KqPQVlPwItE+hcl8C4nU5YJTufOPJnoC5GIyEQChBLlo73XirwjZkFsmdgBQnnxoAeFP0EIO0CJcusWmN+4NhFPspGALW+dv1FGNNPSkaAoMECKZO4Jinm4ubzc9mTibS6CUl4mzqa/yMsAoh9XIMJYuMKVLQAAAABJRU5ErkJggg==');
            width: 16px;
            margin-top: 1px;
            margin-right: 5px;
          }
          .type.method {
            background-image: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsEAAA7BAbiRa+0AAAAadEVYdFNvZnR3YXJlAFBhaW50Lk5FVCB2My41LjExR/NCNwAAAZZJREFUOE+lkbtLQnEcxaVoaOqfCBqicCiIkKCooCATo5bcgyKtwKXMGhoiyHSwbqW45PvVkCJoQehSiZKPpdegVFhmbcF1OHmv3crrL+gxfLjc8z3fc76XKwDwL4giibz/qYGkVwkkLnYux+OrcYTXwl38WcULCW4568qCecZ0se6v8woznyvDtZxZKhwVQHuKeNTl2ZCkPjXIeT7Mry9OpNMuuA48WNv2lCaCGsac8WZAO4ugqTI57QMbcrJy2lYRQMIisSKoCKFgfga9+x6yXcQddQ+tWMeU1LLNudty+3GkdIHPDWrPU9qHwDpsw/l04jPEVETB+IxN6RZUrSomoI7YzGGXOnA+mUBakUZo5hDX1A00/RswiIxQt6o/A5grbjNORGMu+IJuGKzlC8xiy5BNYkdiKomkPAW1cAkmkQnr7RqoWhYha5TVV7XyKX1Gj13iwNlElG3Vd2xivnkBYVmE/Z3EJT7ekf1O5pIl4TKUTUr4Rv193KzK/B2BsUDvbOMcLGLLwFe9wvQXiOJvIIo/B4I3FfjksBM04QcAAAAASUVORK5CYII=');
            width: 16px;
            margin-top: 1px;
            margin-right: 5px;
          }


          .header-block {
            background-color: paleturquoise;
            font-family: Arial, Helvetica, sans-serif;
            width: 75%;
          }
          
          
          table {
            border-collapse: collapse;
            min-width: 75%;
          }
          table, th, td {
            border: 1px solid #CCC;
            padding: 3px;
          }
          td {
            background: #f0f0f0;
            padding-left: 10px;
            padding-right: 10px;
          }
          th {
            background: #e0e0e0;
            text-align: center;
          }

          .hover-text {
            border-bottom: 1px dotted #000;
          }
          
          .helper {
            display: inline-block;
            height: 100%;
            vertical-align: middle;
          }
          
          .cpu {
            text-align: center;
            font-size: small;
          }
          .cpu.graph {
            width: 140px;
            display: inline-table;
            border: 1px solid grey;
            vertical-align: text-bottom;
            float: right;
            margin-top: 1px;
          }
          .cpu.bar {
            height: 10px;
            vertical-align: middle;
            display: table-row;
          }
          .cpu.bar.good {
            background-color: forestgreen;
            display: table-cell;
          }
          .cpu.bar.partial {
            background-color: yellow;
            display: table-cell;
          }
          .cpu.bar.bad {
            background-color: red;
            display: table-cell;
          }
          .cpu.percent {
            padding-right: 10px;
            display: inline-block;
          }
          .cpu-graph-wrapper {
            float: right;
          }

          tr td span.icon {
            background-image: url("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAgAAAAICAYAAADED76LAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMTM0A1t6AAAAK0lEQVQoU2P4//8/XoxVEBljFUTGIAAi8WGsggiMzVhkjFUQGWMVROD/DABjJp9hqefR+wAAAABJRU5ErkJggg==");
            background-position: 50%;
            background-repeat: no-repeat;
            display: inline-block;
            width: 16px;
            height: 16px;
            margin-left: 3px;
            margin-top: 4px;
          }

          tr.collapsed td span.icon {
            background-image: url("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAgAAAAICAYAAADED76LAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMTM0A1t6AAAALElEQVQoU2P4//8/HAMBiAAykcRQOGQpgAniwVgFERjNOLAgihgKh3QF/xkA6ueHeQEpYCgAAAAASUVORK5CYII=");
            background-position: 50%;
            background-repeat: no-repeat;
            display: inline-block;
            width: 16px;
            height: 16px;
            margin-left: 3px;
            margin-top: 4px;
          }

          td.expand-button {
            padding:0px;
          }

          .control-panel {
            position: fixed;
            background-color: rgba(0, 0, 0, 0.2);
            top: 0px;
            right: 0px;
            padding: 1em;
          }
        </style>
        <script type="text/javascript">
          "use strict";

          function parentOf($node) {
            if ($node != null) {
              var parId = $node.data('parent');
              if (parId != null) {
                return $('#' + parId);
              }
            }
            return null;
          }

          function isNodeCollapsed($node) {
            if ($node != null) {
              if ($node.hasClass('collapsed')) {
                return true;
              } else {
                return isNodeCollapsed(parentOf($node));
              }
            }

            return false;
          }

          function evaluateVisibility() {
            var $rows = $('#coverage-table tbody tr');

            // workaround Chromium bug 174167
            // details: https://bugs.chromium.org/p/chromium/issues/detail?id=174167
            var chrome = window.chrome;
            var isChromium = (chrome !== null &amp;&amp; chrome !== undefined);

            $rows.each(function(i, el) {
              var $node = $(el);

              var $parentNode = parentOf($node);
              if (isNodeCollapsed($parentNode)) {
                if (isChromium) {
                  $node.hide();
                } else {
                  $node.css('visibility', 'collapse');
                }
              } else {
                if (isChromium) {
                  $node.show();
                } else {
                  $node.css('visibility', '');
                }
              }
            });
          }

          function toggle(nodeId) {
            console.log('Clicked ' + nodeId);

            var $node = $(nodeId);

            $node.toggleClass('collapsed');

            evaluateVisibility();
          }

          function findParentId($row) {
            var level = $row.data('level');
            var $prevRows = $row.prevAll('tr');

            var parId = null;

            $prevRows.each(function(i, el) {
              var $row = $(el);
              if ($row.data('level') &lt; level) {
                parId = $row.attr('id');
                return false; // stop looping
              }
            });
            
            return parId;
          }

          function toggleAllRows(visible) {
            var $rows = $('#coverage-table tbody tr');

            $rows.each(function(i, el) {
              var $r = $(el);

              if ($r.data('has-children')) {
                $r.toggleClass('collapsed', !visible);
              }
            });

            evaluateVisibility();
          }

          $(document).ready(function() {
            var $rows = $('#coverage-table tbody tr');

            $rows.each(function(i,el) {
              var $r = $(el);
              var parId = findParentId($r);

              if (parId != null) {
                $r.data('parent', parId);
                $('#'+parId).data('has-children', true);
              }
            });

            // run an initial pass to ensure that everything starts sync'd.
            evaluateVisibility();
          });
        </script>
      </head>

      <body>
        <h1><xsl:value-of select="CovProj/Name" /> Coverage Summary</h1>

        <div class="header-block">
          Generation date: <xsl:value-of select="$genDate"/><br/>
          <br/>
          <xsl:variable name="BlocksR" select="number(/CovProj/BlocksCovered) div number($totalBlocks)" />
          <b>Block Coverage:</b> <xsl:value-of select="/CovProj/BlocksCovered"/> of <xsl:value-of select="$totalBlocks"/> (<xsl:value-of select="format-number($BlocksR, '0.00%')" />)<br/>
          <xsl:variable name="LinesR" select="number(/CovProj/LinesCovered) div number($totalLines)" />
          <b>Line Coverage:</b> <xsl:value-of select="/CovProj/LinesCovered"/> of <xsl:value-of select="$totalLines"/> (<xsl:value-of select="format-number($LinesR, '0.00%')" />)<br/>
        </div>

        <br/>
        <br/>

        <table id="coverage-table">
          <thead>
            <tr>
              <th rowspan="2" />
              <th style="text-align:left;" rowspan="2">Name</th>
              <th colspan="5">Line Coverage</th>
              <th colspan="4">Block Coverage</th>
            </tr>
            <tr>
              <th class="cpu" title="Covered">
                <span class="hover-text">C</span>
              </th>
              <th class="cpu" title="Partially Covered">
                <span class="hover-text">P</span>
              </th>
              <th class="cpu" title="Uncovered">
                <span class="hover-text">U</span>
              </th>
              <th class="cpu">
                <span>Total</span>
              </th>
              <th style="width:200px;" />

              <th class="cpu" title="Covered">
                <span class="hover-text">C</span>
              </th>
              <th class="cpu" title="Uncovered">
                <span class="hover-text">U</span>
              </th>
              <th class="cpu">
                <span>Total</span>
              </th>
              <th style="width:200px;" />
            </tr>
          </thead>

          <tbody>
            <xsl:apply-templates select="CovProj" />
          </tbody>
        </table>


        <div class="control-panel">
          <a href="#nowhere" onclick="toggleAllRows(true)">Expand All</a><br/>
          <br/>
          <a href="#nowhere" onclick="toggleAllRows(false)">Collapse All</a>
        </div>
      </body>
    </html>
  </xsl:template>



  <xsl:template match="CovProj">
    <xsl:variable name="hasChildren" select="count(./Modules/*) > 0" />
    <tr id="{generate-id()}" data-level="1">
      <xsl:if test="$hasChildren">
        <xsl:attribute name="class">can-expand expanded</xsl:attribute>
      </xsl:if>
      <td class="expand-button">
        <xsl:if test="$hasChildren">
          <span class="icon" onclick="toggle('#{generate-id()}')" />
        </xsl:if>
      </td>
      <td>
        <div class="type">
          <div class="helper type" />
          <strong>Totals</strong>
        </div>
      </td>
      <xsl:call-template name="CPUData" />
    </tr>

    <xsl:for-each select="Modules">
      <xsl:apply-templates />
    </xsl:for-each>
  </xsl:template>


  <xsl:template match="ModuleExport">
    <xsl:variable name="hasChildren" select="count(./Namespaces/*) > 0" />
    <tr id="{generate-id()}" data-level="2">
      <xsl:if test="$hasChildren">
        <xsl:attribute name="class">can-expand expanded</xsl:attribute>
      </xsl:if>
      <td class="expand-button">
        <xsl:if test="$hasChildren">
          <span class="icon" onclick="toggle('#{generate-id()}')" />
        </xsl:if>
      </td>
      <td class="module-data">
        <div class="type">
          <div class="helper type module" />
          <xsl:value-of select="Name" />
        </div>
      </td>
      <xsl:call-template name="CPUData" />
    </tr>

    <xsl:if test="$depth > 0">
      <xsl:for-each select="Namespaces">
        <xsl:apply-templates />
      </xsl:for-each>
    </xsl:if>
  </xsl:template>


  <xsl:template match="NamespaceExport">
    <xsl:variable name="hasChildren" select="count(./Classes/*) > 0" />
    <tr id="{generate-id()}" data-level="3">
      <xsl:if test="$hasChildren">
        <xsl:attribute name="class">can-expand expanded</xsl:attribute>
      </xsl:if>
      <td class="expand-button">
        <xsl:if test="$hasChildren">
          <span class="icon" onclick="toggle('#{generate-id()}')" />
        </xsl:if>
      </td>
      <td class="namespace-data">
        <div class="type">
          <div class="helper type namespace" />
          <xsl:value-of select="Name" />
        </div>
      </td>
      <xsl:call-template name="CPUData" />
    </tr>

    <xsl:if test="$depth > 1">
      <xsl:for-each select="Classes">
        <xsl:apply-templates />
      </xsl:for-each>
    </xsl:if>
  </xsl:template>


  <xsl:template match="ClassExport">
    <xsl:variable name="hasChildren" select="count(./Methods/*) > 0" />
    <tr id="{generate-id()}" data-level="4">
      <xsl:if test="$hasChildren">
        <xsl:attribute name="class">can-expand collapsed</xsl:attribute>
      </xsl:if>
      <td class="expand-button">
        <xsl:if test="$hasChildren">
          <span class="icon" onclick="toggle('#{generate-id()}')" />
        </xsl:if>
      </td>
      <td class="class-data">
        <div class="type">
          <div class="helper type class" />
          <xsl:value-of select="Name" />
        </div>
      </td>
      <xsl:call-template name="CPUData" />
    </tr>

    <xsl:if test="$depth > 2">
      <xsl:for-each select="Methods">
        <xsl:apply-templates />
      </xsl:for-each>
    </xsl:if>
  </xsl:template>


  <xsl:template match="MethodExport">
    <tr id="{generate-id()}" data-level="5">
      <td />
      <td class="method-data">
        <div class="type">
          <div class="helper type method" />
          <xsl:value-of select="Name" />
        </div>
      </td>
      <xsl:call-template name="CPUData" />
    </tr>
  </xsl:template>


  <xsl:template name="CPUData">
    <xsl:variable name="CLines" select="number(LinesCovered)" />
    <xsl:variable name="PLines" select="number(LinesPartiallyCovered)" />
    <xsl:variable name="ULines" select="number(LinesNotCovered)" />
    <xsl:variable name="TLines" select="LinesCovered + LinesPartiallyCovered + LinesNotCovered" />
    <xsl:variable name="CLinesR" select="round(95 * $CLines div $TLines)" />
    <xsl:variable name="PLinesR" select="round(95 * $PLines div $TLines)" />
    <xsl:variable name="ULinesR" select="round(95 * $ULines div $TLines)" />

    <xsl:variable name="CBlocks" select="BlocksCovered" />
    <xsl:variable name="UBlocks" select="BlocksNotCovered" />
    <xsl:variable name="TBlocks" select="BlocksCovered + BlocksNotCovered" />
    <xsl:variable name="CBlocksR" select="round(95 * $CBlocks div $TBlocks)" />
    <xsl:variable name="UBlocksR" select="round(95 * $UBlocks div $TBlocks)" />
    
    <td class="cpu"><xsl:value-of select="$CLines" /></td>
    <td class="cpu"><xsl:value-of select="$PLines" /></td>
    <td class="cpu"><xsl:value-of select="$ULines" /></td>
    <td class="cpu"><xsl:value-of select="$TLines" /></td>
    <td>
      <div class="cpu-graph-wrapper">
        <div class="cpu percent">
          <xsl:value-of select="format-number($CLines div $TLines, '0.00%')"/>
        </div>
        <div class="cpu graph">
          <div class="cpu bar">
            <xsl:if test="$CLines > 0">
              <div class="cpu bar good" style="width:{$CLinesR}%;" />
            </xsl:if>
            <xsl:if test="$PLines > 0">
              <div class="cpu bar partial" style="width:{$PLinesR}%" />
            </xsl:if>
            <xsl:if test="$ULines > 0">
              <div class="cpu bar bad" style="width:{$ULinesR}%" />
            </xsl:if>
          </div>
        </div>
      </div>
    </td>

    <td class="cpu"><xsl:value-of select="$CBlocks" /></td>
    <td class="cpu"><xsl:value-of select="$UBlocks" /></td>
    <td class="cpu"><xsl:value-of select="$TBlocks" /></td>
    <td>
      <div class="cpu-graph-wrapper">
        <div class="cpu percent">
          <xsl:value-of select="format-number($CBlocks div $TBlocks, '0.00%')"/>
        </div>
        <div class="cpu graph">
          <div class="cpu bar">
            <xsl:if test="$CBlocks > 0">
              <div class="cpu bar good" style="width:{$CBlocksR}%" />
            </xsl:if>
            <xsl:if test="$UBlocks > 0">
              <div class="cpu bar bad" style="width:{$UBlocksR}%" />
            </xsl:if>
          </div>
        </div>
      </div>
    </td>
  </xsl:template>

</xsl:stylesheet>
